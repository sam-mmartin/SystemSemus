using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SistemaSemus.DAL;
using SistemaSemus.Models;
using System.Data.Entity.Infrastructure;
using System.Net;
using SistemaSemus.ViewModels;
using Microsoft.AspNet.Identity;

namespace SistemaSemus.Controllers
{
    public class PedidoEstoqueController : Controller
    {
        private readonly SemusContext db = new SemusContext();

        public async Task<ActionResult> Index(int estoqueID, byte tipoProduto)
        {
            List<PedidoEstoque> pedidos = new List<PedidoEstoque>();
            pedidos = estoqueID == 1
                ? await db.PedidoEstoques.Where(p => p.TipoPedido == tipoProduto).OrderByDescending(p => p.ID).ToListAsync()
                : await db.PedidoEstoques.Where(p => p.EstoqueSemusID == estoqueID && p.TipoPedido == tipoProduto).OrderByDescending(p => p.ID).ToListAsync();

            ViewBag.Estoque = estoqueID;
            ViewBag.Descricao = db.EstoqueSemus.Find(estoqueID).Descricao;
            return View(pedidos);
        }

        public ActionResult Create(int estoqueID, byte TipoProduto)
        {
            ViewBag.Produtos = new SelectList(db.ProdutoEstoqueSemus
                .Where(m => m.EstoqueSemus_ID == 1 && m.Produto.TipoProduto == TipoProduto)
                .Select(p => p.Produto), "ID", "Descricao");

            ViewBag.ProdutosSemEstoque = db.ProdutoNaoCadatrados.Count() == 0
                ? null
                : new SelectList(db.ProdutoNaoCadatrados.Where(m => m.TipoProduto == TipoProduto), "ID", "Descricao");

            PedidoEstoque novoPedido = new PedidoEstoque
            {
                EstoqueSemusID = estoqueID,
                TipoPedido = TipoProduto,
                Faturado = false
            };

            return View(novoPedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EstoqueSemusID, TipoPedido, Faturado")] PedidoEstoque pedidoEstoque, string[] produtos, string[] produtos2, string[] produtos3)
        {
            try
            {
                if (produtos == null && produtos2 == null && produtos3 == null)
                {
                    ViewBag.Produtos = new SelectList(db.ProdutoEstoqueSemus
                        .Where(m => m.EstoqueSemus_ID == 1 && m.Produto.TipoProduto == pedidoEstoque.TipoPedido)
                        .Select(p => p.Produto), "ID", "Descricao");

                    ViewBag.ProdutosSemEstoque = db.ProdutoNaoCadatrados.Count() == 0
                        ? null
                        : new SelectList(db.ProdutoNaoCadatrados.Where(m => m.TipoProduto == pedidoEstoque.TipoPedido), "ID", "Descricao");

                    TempData["Message"] = "Nenhum produto foi adicionado ao carrinho!";
                    return View(pedidoEstoque);
                }

                if (ModelState.IsValid)
                {
                    pedidoEstoque.PedidoProdutos = new List<PedidoProduto>();

                    // Produtos Cadastrados
                    if (produtos != null)
                    {
                        var todosProdutos = await db.ProdutoEstoqueSemus
                            .Where(p => p.EstoqueSemus_ID == 1 && p.Produto.TipoProduto == pedidoEstoque.TipoPedido)
                            .ToListAsync();

                        foreach (var item in produtos)
                        {
                            string[] dados = item.Split('/');
                            int itemID = int.Parse(dados[0]);
                            var itemProduto = todosProdutos.SingleOrDefault(p => p.Produto_ID == itemID);

                            var novoProduto = new PedidoProduto
                            {
                                PedidoEstoqueID = pedidoEstoque.ID,
                                ProdutoID = itemProduto.Produto_ID,
                                Descricao = itemProduto.Produto.Descricao,
                                Quantidade = int.Parse(dados[1])
                            };
                            pedidoEstoque.PedidoProdutos.Add(novoProduto);
                        }
                    }

                    // Produtos Não Cadastrados
                    if (produtos2 != null || produtos3 != null)
                    {
                        var todosProdutosSemEstoque = await db.ProdutoNaoCadatrados
                                   .Where(p => p.TipoProduto == pedidoEstoque.TipoPedido)
                                   .ToListAsync();

                        if (produtos2 != null)
                        {
                            foreach (var item in produtos2)
                            {
                                string[] dados = item.Split('|');
                                var produtoNaoCadastrado = todosProdutosSemEstoque
                                    .SingleOrDefault(p => p.Descricao.Equals(dados[0]));

                                var novoProduto = new PedidoProduto
                                {
                                    PedidoEstoqueID = pedidoEstoque.ID,
                                    Descricao = dados[0].ToUpper(),
                                    Quantidade = int.Parse(dados[1])
                                };

                                if (produtoNaoCadastrado != null)
                                {
                                    novoProduto.ProdutoSemEstoqueID = produtoNaoCadastrado.ID;
                                }
                                else
                                {
                                    var novoProdutoNaoCadastrado = new ProdutoNaoCadastrado
                                    {
                                        TipoProduto = pedidoEstoque.TipoPedido,
                                        Quantidade = novoProduto.Quantidade,
                                        Descricao = novoProduto.Descricao,
                                        DataEntrada = DateTime.Now
                                    };

                                    int produtoNaoCadastradoID = await MetodosGlobais.Compras.NovoProdutoNaoCadastrado(novoProdutoNaoCadastrado);
                                    novoProduto.ProdutoSemEstoqueID = produtoNaoCadastradoID;
                                }
                                pedidoEstoque.PedidoProdutos.Add(novoProduto);
                            }
                        }

                        if (produtos3 != null)
                        {
                            foreach (var item in produtos3)
                            {
                                string[] dados = item.Split('-');
                                int idProduto = int.Parse(dados[0]);
                                var produtoNaoCadastrado = todosProdutosSemEstoque
                                    .SingleOrDefault(p => p.ID == idProduto);

                                var novoProduto = new PedidoProduto
                                {
                                    PedidoEstoqueID = pedidoEstoque.ID,
                                    Descricao = produtoNaoCadastrado.Descricao,
                                    ProdutoSemEstoqueID = produtoNaoCadastrado.ID,
                                    Quantidade = int.Parse(dados[1])
                                };
                                pedidoEstoque.PedidoProdutos.Add(novoProduto);
                            }
                        }
                    }

                    pedidoEstoque.UserID = User.Identity.GetUserId();
                    pedidoEstoque.DataEntrada = DateTime.Now;

                    _ = db.PedidoEstoques.Add(pedidoEstoque);
                    _ = await db.SaveChangesAsync();

                    TempData["Message"] = "Novo Pedido realizado com sucesso.";
                    RedirectToRouteResult redirectToRouteResult = RedirectToAction("Details",
                                                                                   "EstoqueSemus",
                                                                                   new { id = pedidoEstoque.EstoqueSemusID, tipoProduto = pedidoEstoque.TipoPedido });
                    return redirectToRouteResult;
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                            "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
            }

            return RedirectToAction("Create",
                                    new { estoqueID = pedidoEstoque.EstoqueSemusID, TipoProduto = pedidoEstoque.TipoPedido });
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Número do pedido não informado. Tente novamente e, se o problema persistir, consulte o administrador do sistema.";
                return RedirectToAction("Details", "EstoqueSemus", new { id = 1 });
            }
            var pedidoEstoque = await db.PedidoEstoques.FindAsync(id);
            if (pedidoEstoque == null)
            {
                TempData["Message"] = "Pedido não encontrado. Verifique se foi informado um número de pedido correto.";
                return RedirectToAction("Details", "EstoqueSemus", new { id = 1 });
            }

            if (pedidoEstoque.Faturado)
            {
                TempData["Message"] = "Este Pedido já foi faturado.";
                return RedirectToAction("Details", "EstoqueSemus", new { id = 1 });
            }
            return RedirectToAction("FaturarPedido", new { id });
        }

        public async Task<ActionResult> FaturarPedido(int id)
        {
            PedidoEstoque pedidoEstoque = await db.PedidoEstoques.FindAsync(id);
            ViewBag.Estoque = 1;
            ViewBag.Descricao = db.EstoqueSemus.Find(1).Descricao;
            return View(pedidoEstoque);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FaturarPedido(int? id, string[] produtos)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var pedidoEstoque = await db.PedidoEstoques.FindAsync(id);

                if (produtos != null)
                {
                    #region Variaveis
                    var todosProdutos = await db.ProdutoEstoqueSemus
                        .Where(p => p.Produto.TipoProduto == pedidoEstoque.TipoPedido && p.EstoqueSemus_ID == 1)
                        .ToListAsync();

                    var produtoSemEstoque = await db.ProdutoNaoCadatrados
                        .Where(p => p.TipoProduto == pedidoEstoque.TipoPedido)
                        .ToListAsync();

                    var produtosCompra = new ProdutosCompra
                    {
                        PedidoProdutos = new List<PedidoProduto>(),
                        ProdutoNaoCadastrados = new List<ProdutoNaoCadastrado>()
                    };

                    string userID = User.Identity.GetUserId();
                    bool verificaCompra = false;
                    #endregion

                    foreach (string item in produtos)
                    {
                        int idProduto = int.Parse(item);
                        var produtoEncontrado = new ProdutoEstoqueSemus();
                        var itemPedido = pedidoEstoque
                            .PedidoProdutos.SingleOrDefault(p => p.ID == idProduto);

                        if (itemPedido != null)
                        {
                            // Verifica se o item está registrado no Banco.
                            // Caso sim, verifica se existe a quantidade pedida é maior que a disponível.
                            // Se é menor ou igual, ou se a quantidade do produto está zerada.
                            if (itemPedido.ProdutoID != null)
                            {
                                var produtoGeral = todosProdutos.SingleOrDefault(p => p.Produto_ID == itemPedido.ProdutoID);

                                switch (produtoGeral.QuantidadeTotal)
                                {
                                    case 0:
                                        produtoGeral.QuantidadeEmFalta += itemPedido.Quantidade;
                                        produtoGeral.UserID = userID;

                                        db.Entry(produtoGeral).State = EntityState.Modified;
                                        produtosCompra.PedidoProdutos.Add(itemPedido);
                                        verificaCompra = true;
                                        break;
                                    default:
                                        if (produtoGeral.QuantidadeTotal >= itemPedido.Quantidade)
                                        {
                                            var itemEstoque = itemPedido
                                                .PedidoEstoque.EstoqueSemus.ProdutoEstoqueSemus
                                                .SingleOrDefault(p => p.Produto_ID == itemPedido.ProdutoID);

                                            switch (itemEstoque)
                                            {
                                                case null:
                                                    {
                                                        // Adiciona o Produto no estoque da Unidade
                                                        var novoItemEstoque = new ProdutoEstoqueSemus
                                                        {
                                                            Produto_ID = Convert.ToInt32(itemPedido.ProdutoID),
                                                            EstoqueSemus_ID = itemPedido.PedidoEstoque.EstoqueSemusID,
                                                            QuantidadeTotal = itemPedido.Quantidade,
                                                            QuantidadeEntrada = itemPedido.Quantidade,
                                                            QuantidadeSaida = 0,
                                                            DataEntrada = DateTime.Now,
                                                            UserID = userID
                                                        };
                                                        itemPedido.PedidoEstoque.EstoqueSemus.ProdutoEstoqueSemus.Add(novoItemEstoque);
                                                        // Decrementa a quantidade do produto no Estoque Geral
                                                        produtoGeral.QuantidadeTotal -= itemPedido.Quantidade;
                                                        produtoGeral.QuantidadeSaida = itemPedido.Quantidade;
                                                        produtoGeral.DataSaida = DateTime.Now;
                                                        produtoGeral.UserID = userID;
                                                        break;
                                                    }

                                                default:
                                                    // Incrementa a quantidade do Produto no estoque da Unidade
                                                    itemEstoque.QuantidadeEntrada = itemPedido.Quantidade;
                                                    itemEstoque.QuantidadeTotal += itemPedido.Quantidade;
                                                    itemEstoque.DataEntrada = DateTime.Now;
                                                    itemEstoque.UserID = userID;
                                                    db.Entry(itemEstoque).State = EntityState.Modified;
                                                    // Decrementa a quantidade do produto no Estoque Geral
                                                    produtoGeral.QuantidadeTotal -= itemPedido.Quantidade;
                                                    produtoGeral.QuantidadeSaida = itemPedido.Quantidade;
                                                    produtoGeral.DataSaida = DateTime.Now;
                                                    produtoGeral.UserID = userID;
                                                    break;
                                            }

                                            db.Entry(produtoGeral).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            var itemEstoque = pedidoEstoque
                                                .EstoqueSemus.ProdutoEstoqueSemus
                                                .SingleOrDefault(p => p.Produto_ID == itemPedido.ProdutoID);

                                            switch (itemEstoque)
                                            {
                                                case null:
                                                    {
                                                        // Adiciona o Produto no estoque da Unidade
                                                        var novoItemEstoque = new ProdutoEstoqueSemus
                                                        {
                                                            Produto_ID = Convert.ToInt32(itemPedido.ProdutoID),
                                                            EstoqueSemus_ID = itemPedido.PedidoEstoque.EstoqueSemusID,
                                                            QuantidadeTotal = produtoGeral.QuantidadeTotal,
                                                            QuantidadeEntrada = produtoGeral.QuantidadeTotal,
                                                            QuantidadeSaida = 0,
                                                            DataEntrada = DateTime.Now,
                                                            UserID = userID
                                                        };
                                                        itemPedido.PedidoEstoque.EstoqueSemus.ProdutoEstoqueSemus.Add(novoItemEstoque);
                                                        // Decrementa a quantidade do produto no Estoque Geral
                                                        produtoGeral.QuantidadeSaida = produtoGeral.QuantidadeTotal;
                                                        produtoGeral.QuantidadeEmFalta = itemPedido.Quantidade - produtoGeral.QuantidadeTotal;
                                                        produtoGeral.QuantidadeTotal = 0;
                                                        produtoGeral.DataSaida = DateTime.Now;
                                                        produtoGeral.UserID = userID;
                                                        break;
                                                    }

                                                default:
                                                    // Incrementa a quantidade do Produto no estoque da Unidade
                                                    itemEstoque.QuantidadeEntrada = produtoGeral.QuantidadeTotal;
                                                    itemEstoque.QuantidadeTotal += produtoGeral.QuantidadeTotal;
                                                    itemEstoque.DataEntrada = DateTime.Now;
                                                    itemEstoque.UserID = userID;
                                                    db.Entry(itemEstoque).State = EntityState.Modified;
                                                    // Decrementa a quantidade do produto no Estoque Geral
                                                    produtoGeral.QuantidadeSaida = produtoGeral.QuantidadeTotal;
                                                    produtoGeral.QuantidadeEmFalta = itemPedido.Quantidade - produtoGeral.QuantidadeTotal;
                                                    produtoGeral.QuantidadeTotal = 0;
                                                    produtoGeral.DataSaida = DateTime.Now;
                                                    produtoGeral.UserID = userID;
                                                    break;
                                            }

                                            db.Entry(produtoGeral).State = EntityState.Modified;
                                            itemPedido.Quantidade -= produtoGeral.QuantidadeTotal;
                                            produtosCompra.PedidoProdutos.Add(itemPedido);
                                            verificaCompra = true;
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                // Verifica se o itemPedido possui um ID de um produto não cadastrado.
                                switch (itemPedido.ProdutoSemEstoqueID)
                                {
                                    // Caso não possua, realiza a busca utilizando como referencia a descrição do itemPedido
                                    // Caso possua, realiza a busca utilizando o dado contido em itemPedido
                                    case null:
                                        {
                                            var produtoNaoCadastrado = produtoSemEstoque
                                                .SingleOrDefault(p => p.Descricao.Equals(itemPedido.Descricao));

                                            // Verifica se existe um registro
                                            switch (produtoNaoCadastrado)
                                            {
                                                // Adiciona um novo registro à tabela de não cadastrados utlizando os dados de itemPedido
                                                // Caso encontre um registro, realiza o Update do objeto encontrado.
                                                case null:
                                                    {
                                                        var novoProdutoSemEstoque = new ProdutoNaoCadastrado
                                                        {
                                                            TipoProduto = pedidoEstoque.TipoPedido,
                                                            Quantidade = itemPedido.Quantidade,
                                                            Descricao = itemPedido.Descricao,
                                                            DataEntrada = DateTime.Now,
                                                            DataPedido = DateTime.Now
                                                        };

                                                        int produtoNaoCadastradoID = await MetodosGlobais.Compras.NovoProdutoNaoCadastrado(novoProdutoSemEstoque);
                                                        novoProdutoSemEstoque = await db.ProdutoNaoCadatrados.FindAsync(produtoNaoCadastradoID);
                                                        produtosCompra.ProdutoNaoCadastrados.Add(novoProdutoSemEstoque);
                                                        verificaCompra = true;
                                                        break;
                                                    }

                                                default:
                                                    produtoNaoCadastrado.Quantidade += itemPedido.Quantidade;
                                                    produtoNaoCadastrado.DataPedido = DateTime.Now;
                                                    db.Entry(produtoNaoCadastrado).State = EntityState.Modified;
                                                    produtosCompra.ProdutoNaoCadastrados.Add(produtoNaoCadastrado);
                                                    verificaCompra = true;
                                                    break;
                                            }

                                            break;
                                        }

                                    default:
                                        {
                                            var produtoNaoCadastrado = produtoSemEstoque
                                                .SingleOrDefault(p => p.ID == itemPedido.ProdutoSemEstoqueID);

                                            // Verifica se existe um registro
                                            switch (produtoNaoCadastrado)
                                            {
                                                // Caso não, realiza uma busca na tabela de Produtos utilizando a descrição contida em itemPedido
                                                // Caso sim, realiza o Update do Objeto encontrado.
                                                case null:
                                                    {
                                                        var searchProdutos = todosProdutos
                                                            .Where(p => p.Produto.Descricao.Contains(itemPedido.Descricao))
                                                            .ToList();

                                                        // Verifica se foram encontrados itens com a descrição semelhante a do itemPedido
                                                        switch (searchProdutos.Count)
                                                        {
                                                            // Caso não encontre, através de um método externo realiza os procedimentos das linhas 356 a 395
                                                            // O metodo retorna um objeto BooleanCompra, que modifica a variavel verificaCompra 
                                                            // e, adiciona um produto não cadastrado a lista de produtosCompra
                                                            case 0:
                                                                {
                                                                    var booleanCompra = await MetodosGlobais.ExecutarUpdateEstoque.UpdateNaoCadastrado(itemPedido);
                                                                    if (booleanCompra.Verifica)
                                                                    {
                                                                        verificaCompra = booleanCompra.Verifica;
                                                                        var adicionaPNCadastrado = await db.ProdutoNaoCadatrados.FindAsync(booleanCompra.Quantidade);
                                                                        produtosCompra.ProdutoNaoCadastrados.Add(adicionaPNCadastrado);
                                                                    }

                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    // Caso encontre itens que batam com a descrição pesquisada
                                                                    // Realiza um laço verificando se algum elemento da lista corresponde à descrição de itemPedido
                                                                    foreach (var search in searchProdutos)
                                                                    {
                                                                        if (search.Produto.Descricao.Equals(itemPedido.Descricao))
                                                                        {
                                                                            // Caso encontre o metodo VerificarIncrementar(ProdutoEstoqueSemus, PedidoProduto) recebe o itemPedido e o elemento correspondente
                                                                            // O método realiza os passos das linhas 241 a 248.
                                                                            // E, retorna a classe BooleanCompra que possui um atributo bool e um atributo int
                                                                            var booleanCompra = await MetodosGlobais.ExecutarUpdateEstoque.VerificarIncrementar(search, itemPedido, userID);
                                                                            if (booleanCompra.Verifica)
                                                                            {
                                                                                if (booleanCompra.Quantidade != 0)
                                                                                {
                                                                                    itemPedido.Quantidade = booleanCompra.Quantidade;
                                                                                }
                                                                                itemPedido.ProdutoID = search.Produto_ID;
                                                                                produtosCompra.PedidoProdutos.Add(itemPedido);
                                                                                verificaCompra = booleanCompra.Verifica;
                                                                            }
                                                                            break;
                                                                        }
                                                                    }

                                                                    break;
                                                                }
                                                        }
                                                        break;
                                                    }

                                                default:
                                                    produtoNaoCadastrado.Quantidade += itemPedido.Quantidade;
                                                    produtoNaoCadastrado.DataPedido = DateTime.Now;
                                                    db.Entry(produtoNaoCadastrado).State = EntityState.Modified;
                                                    produtosCompra.ProdutoNaoCadastrados.Add(produtoNaoCadastrado);
                                                    verificaCompra = true;
                                                    break;
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }

                    // Verifica se existe algum caso que necessite realizar um pedido de compra
                    if (verificaCompra)
                    {
                        // Cria um novo objeto PedidoCompra e retorna o ID do objeto
                        // Realiza a busca do objeto.
                        int pedidoCompraID = await MetodosGlobais.Compras.NovaCompra(pedidoEstoque.TipoPedido);
                        var novaCompra = await db.PedidoCompras.FindAsync(pedidoCompraID);

                        // Adiciona os produtos à nova compra
                        if (produtosCompra.PedidoProdutos.Count != 0)
                        {
                            novaCompra.PedidoCompraProdutos = new List<PedidoCompraProduto>();
                            foreach (var item in produtosCompra.PedidoProdutos)
                            {
                                var novoCompraProduto = new PedidoCompraProduto
                                {
                                    Produto_ID = Convert.ToInt32(item.ProdutoID),
                                    PedidoCompra_ID = pedidoCompraID,
                                    Quantidade = item.Quantidade
                                };
                                novaCompra.PedidoCompraProdutos.Add(novoCompraProduto);
                            }
                        }
                        // Adiciona os produtos não cadastrados à nova compra
                        if (produtosCompra.ProdutoNaoCadastrados.Count != 0)
                        {
                            novaCompra.PedidoCompraNaoCadastrados = new List<PedidoCompraNaoCadastrado>();
                            foreach (var item in produtosCompra.ProdutoNaoCadastrados)
                            {
                                var novoCompraProduto = new PedidoCompraNaoCadastrado
                                {
                                    Produto_ID = item.ID,
                                    PedidoCompra_ID = novaCompra.ID,
                                    Quantidade = item.Quantidade
                                };
                                novaCompra.PedidoCompraNaoCadastrados.Add(novoCompraProduto);
                            }
                        }

                        novaCompra.UserID = userID;
                        db.Entry(novaCompra).State = EntityState.Modified;
                    }
                }

                pedidoEstoque.DataFaturado = DateTime.Now;
                pedidoEstoque.Faturado = true;
                pedidoEstoque.UserID = User.Identity.GetUserId();

                db.Entry(pedidoEstoque).State = EntityState.Modified;
                _ = await db.SaveChangesAsync();

                TempData["Message"] = "Pedido faturado com sucesso.";
                return RedirectToAction("Details", "EstoqueSemus", new { id = 1 });
            }
            catch (RetryLimitExceededException)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. "
                                      + "Tente novamente e, se o problema persistir, consulte o administrador do sistema.";
                return RedirectToAction("FaturarPedido", new { id });
            }
        }

        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PedidoEstoque pedidoEstoque = await db.PedidoEstoques.FindAsync(id);
        //    if (pedidoEstoque == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(pedidoEstoque);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "ID,DataEntrada,EstoqueSemusID")] PedidoEstoque pedidoEstoque)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(pedidoEstoque).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(pedidoEstoque);
        //}

        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    PedidoEstoque pedidoEstoque = await db.PedidoEstoques.FindAsync(id);
        //    if (pedidoEstoque == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(pedidoEstoque);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    PedidoEstoque pedidoEstoque = await db.PedidoEstoques.FindAsync(id);
        //    db.PedidoEstoques.Remove(pedidoEstoque);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}