using Microsoft.AspNet.Identity;
using SistemaSemus.DAL;
using SistemaSemus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SistemaSemus.Controllers
{
    public class PedidoCompraController : Controller
    {
        private readonly SemusContext db = new SemusContext();

        public async Task<ActionResult> Index(byte? tipoProduto)
        {
            ViewBag.Estoque = 1;
            ViewBag.Descricao = db.EstoqueSemus.Find(1).Descricao;

            if (tipoProduto == null)
            {
                ViewBag.UnirCompras = false;
                ViewBag.TipoProduto = "Todas Ordem de Compras";
                return View(await db.PedidoCompras.OrderByDescending(p => p.ID).ToListAsync());
            }
            else
            {
                ViewBag.UnirCompras = true;
                ViewBag.TipoProduto = tipoProduto == 1 ? "Medicamentos" : "Suprimentos";
                return View(await db.PedidoCompras.Where(p => p.TipoProduto == tipoProduto).OrderByDescending(p => p.ID).ToListAsync());
            }
        }

        public ActionResult Create(byte tipoProduto)
        {
            ViewBag.Produtos = new SelectList(db.Produtos
                .Where(m => m.TipoProduto == tipoProduto)
                .Select(p => p), "ID", "Descricao");

            ViewBag.ProdutosSemEstoque = db.ProdutoNaoCadatrados.Count() == 0
                ? null
                : new SelectList(db.ProdutoNaoCadatrados.Where(m => m.TipoProduto == tipoProduto), "ID", "Descricao");

            var novaCompra = new PedidoCompra
            {
                EstoqueSemus_ID = 1,
                TipoProduto = tipoProduto,
                Faturado = false
            };
            return View(novaCompra);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EstoqueSemus_ID,TipoProduto,Faturado")] PedidoCompra pedidoCompra, string[] produtos, string[] produtos2, string[] produtos3)
        {
            try
            {
                if (produtos == null && produtos2 == null && produtos3 == null)
                {
                    ViewBag.Produtos = new SelectList(db.Produtos
                        .Where(m => m.TipoProduto == pedidoCompra.TipoProduto)
                        .Select(p => p), "ID", "Descricao");

                    ViewBag.ProdutosSemEstoque = db.ProdutoNaoCadatrados.Count() == 0
                        ? null
                        : new SelectList(db.ProdutoNaoCadatrados.Where(m => m.TipoProduto == pedidoCompra.TipoProduto), "ID", "Descricao");

                    TempData["Message"] = "Nenhum produto foi adicionado ao carrinho!";
                    return View(pedidoCompra);
                }

                if (ModelState.IsValid)
                {
                    // Produtos Cadastrados
                    if (produtos != null)
                    {
                        pedidoCompra.PedidoCompraProdutos = new List<PedidoCompraProduto>();
                        var todosProdutos = await db.ProdutoEstoqueSemus
                            .Where(p => p.EstoqueSemus_ID == 1 && p.Produto.TipoProduto == pedidoCompra.TipoProduto)
                            .ToListAsync();

                        foreach (var item in produtos)
                        {
                            string[] dados = item.Split('/');
                            int itemID = int.Parse(dados[0]);
                            var itemProduto = todosProdutos.SingleOrDefault(p => p.Produto_ID == itemID);

                            if (itemProduto != null)
                            {
                                var novoProduto = new PedidoCompraProduto
                                {
                                    PedidoCompra_ID = pedidoCompra.ID,
                                    Produto_ID = itemProduto.Produto_ID,
                                    Quantidade = int.Parse(dados[1])
                                };
                                pedidoCompra.PedidoCompraProdutos.Add(novoProduto);
                            }
                        }
                    }

                    // Produtos não cadastrados
                    if (produtos2 != null || produtos3 != null)
                    {
                        pedidoCompra.PedidoCompraNaoCadastrados = new List<PedidoCompraNaoCadastrado>();
                        var todosProdutosSemEstoque = await db.ProdutoNaoCadatrados
                                .Where(p => p.TipoProduto == pedidoCompra.TipoProduto)
                                .ToListAsync();

                        if (produtos2 != null)
                        {
                            foreach (var item in produtos2)
                            {
                                string[] dados = item.Split('|');
                                var produtoNaoCadastrado = todosProdutosSemEstoque
                                    .SingleOrDefault(p => p.Descricao.Equals(dados[0]));

                                if (produtoNaoCadastrado != null)
                                {
                                    var novoProduto = new PedidoCompraNaoCadastrado
                                    {
                                        PedidoCompra_ID = pedidoCompra.ID,
                                        Produto_ID = produtoNaoCadastrado.ID,
                                        Quantidade = int.Parse(dados[1])
                                    };
                                    pedidoCompra.PedidoCompraNaoCadastrados.Add(novoProduto);
                                }
                                else
                                {
                                    var novoProdutoNaoCadastrado = new ProdutoNaoCadastrado
                                    {
                                        TipoProduto = pedidoCompra.TipoProduto,
                                        Quantidade = int.Parse(dados[1]),
                                        Descricao = dados[0],
                                        DataEntrada = DateTime.Now
                                    };

                                    int produtoNaoCadastradoID = await MetodosGlobais.Compras.NovoProdutoNaoCadastrado(novoProdutoNaoCadastrado);

                                    var novoproduto = new PedidoCompraNaoCadastrado
                                    {
                                        PedidoCompra_ID = pedidoCompra.ID,
                                        Produto_ID = produtoNaoCadastradoID,
                                        Quantidade = int.Parse(dados[1])
                                    };
                                    pedidoCompra.PedidoCompraNaoCadastrados.Add(novoproduto);
                                }
                            }
                        }

                        if (produtos3 != null)
                        {
                            foreach (var item in produtos3)
                            {
                                string[] dados = item.Split('-');
                                int produtoID = int.Parse(dados[0]);
                                var produtoNaoCadastrado = todosProdutosSemEstoque.SingleOrDefault(p => p.ID == produtoID);

                                if (produtoNaoCadastrado != null)
                                {
                                    var novoProduto = new PedidoCompraNaoCadastrado
                                    {
                                        Produto_ID = produtoID,
                                        PedidoCompra_ID = pedidoCompra.ID,
                                        Quantidade = int.Parse(dados[1])
                                    };
                                    pedidoCompra.PedidoCompraNaoCadastrados.Add(novoProduto);
                                }
                            }
                        }
                    }

                    pedidoCompra.UserID = User.Identity.GetUserId();
                    pedidoCompra.DataEntrada = DateTime.Now;

                    _ = db.PedidoCompras.Add(pedidoCompra);
                    _ = await db.SaveChangesAsync();

                    TempData["Message"] = "Novo pedido realizado com sucesso.";
                    return RedirectToAction("Index", new { tipoProduto = pedidoCompra.TipoProduto });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                            "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
            }

            return View(pedidoCompra);
        }

        public async Task<ActionResult> UnirCompras(byte tipoProduto, string[] pedidos)
        {
            try
            {
                if (pedidos != null)
                {
                    if (pedidos.Length > 1)
                    {
                        var pedidosSelecionados = new List<PedidoCompra>();

                        foreach (var item in pedidos)
                        {
                            int pedidoID = int.Parse(item);
                            var pedidoCompra = await db.PedidoCompras.FindAsync(pedidoID);
                            if (!pedidoCompra.Faturado)
                            {
                                pedidosSelecionados.Add(pedidoCompra);
                            }
                        }

                        bool v = pedidosSelecionados.Count != 0 && pedidosSelecionados.Count > 1;
                        if (v)
                        {
                            int novaCompraID = await MetodosGlobais.Compras.NovaCompra(tipoProduto);
                            var pedidoCompra = await db.PedidoCompras.FindAsync(novaCompraID);

                            foreach (var item in pedidosSelecionados)
                            {
                                if (item.PedidoCompraProdutos.Count > 0)
                                {
                                    var pedidos_compras_produtos_deleted = new List<PedidoCompraProduto>();

                                    foreach (var elemento in item.PedidoCompraProdutos)
                                    {
                                        var novoProdutoCompra = new PedidoCompraProduto
                                        {
                                            Produto_ID = elemento.Produto_ID,
                                            Quantidade = elemento.Quantidade
                                        };

                                        pedidoCompra.PedidoCompraProdutos.Add(novoProdutoCompra);
                                        pedidos_compras_produtos_deleted.Add(elemento);
                                    }

                                    foreach (var deleted in pedidos_compras_produtos_deleted)
                                    {
                                        _ = db.PedidoCompraProdutos.Remove(deleted);
                                    }
                                }

                                if (item.PedidoCompraNaoCadastrados.Count > 0)
                                {
                                    var pedidos_compras_naoCadastrado_deleted = new List<PedidoCompraNaoCadastrado>();

                                    foreach (var elemento in item.PedidoCompraNaoCadastrados)
                                    {
                                        var novoProdutoCompra = new PedidoCompraNaoCadastrado
                                        {
                                            Produto_ID = elemento.Produto_ID,
                                            Quantidade = elemento.Quantidade
                                        };

                                        pedidoCompra.PedidoCompraNaoCadastrados.Add(novoProdutoCompra);
                                        pedidos_compras_naoCadastrado_deleted.Add(elemento);
                                    }

                                    foreach (var deleted in pedidos_compras_naoCadastrado_deleted)
                                    {
                                        _ = db.PedidoCompraNaoCadastrados.Remove(deleted);
                                    }
                                }

                                var deletedPedidoCompra = await db.PedidoCompras.FindAsync(item.ID);
                                _ = db.PedidoCompras.Remove(deletedPedidoCompra);
                            }

                            pedidoCompra.UserID = User.Identity.GetUserId();
                            db.Entry(pedidoCompra).State = EntityState.Modified;
                            _ = await db.SaveChangesAsync();

                            TempData["Message"] = "Realizada junção de pedidos de compra!";
                            return RedirectToAction("Index", new { tipoProduto });
                        }
                        else
                        {
                            TempData["Message"] = "Alguns pedidos já foram faturados!";
                            return RedirectToAction("Index", new { tipoProduto });
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Selecione um mínimo de 2 pedidos de compra!";
                        return RedirectToAction("Index", new { tipoProduto });
                    }
                }
                else
                {
                    TempData["Message"] = "Selecione um mínimo de 2 pedidos de compra!";
                    return RedirectToAction("Index", new { tipoProduto });
                }
            }
            catch (Exception ex)
            {
                return ex.InnerException == null
                    ? RedirectToAction("Erro", "Home", new { Erro = ex.Message })
                    : RedirectToAction("Erro", "Home", new { Erro = ex.InnerException.Message });
            }
        }

        public async Task<ActionResult> FaturarCompra(int? id, byte tipoProduto)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoCompra pedidoCompra = await db.PedidoCompras.FindAsync(id);
            if (pedidoCompra == null)
            {
                TempData["Message"] = "Ordem de Compra não encontrada. " +
                    "Tente novamente, se o problema persistir, entre em contato com administrador do sistema";
                return RedirectToAction("Index", new { tipoProduto });
            }

            ViewBag.Estoque = pedidoCompra.EstoqueSemus_ID;
            ViewBag.Descricao = pedidoCompra.EstoqueSemus.Descricao;
            return View(pedidoCompra);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FaturarCompra(int? id, string[] produtos, string[] notProdutos)
        {
            try
            {
                if (id == null)
                {
                    TempData["Message"] = "A ordem de compra não foi encontrada. " +
                        "O registro foi apagado por outro usuário. Entre em contato com administrador do sistema.";
                    return RedirectToAction("Index");
                }

                var pedidoCompra = await db.PedidoCompras.FindAsync(id);

                if (pedidoCompra == null)
                {
                    TempData["Message"] = "A ordem de compra não foi encontrada. " +
                        "O registro foi apagado por outro usuário. Entre em contato com administrador do sistema.";
                    return RedirectToAction("Index");
                }

                var userID = User.Identity.GetUserId();

                if (produtos != null)
                {
                    foreach (var item in produtos)
                    {
                        int idProduto = int.Parse(item);
                        var produtoCompra = pedidoCompra.PedidoCompraProdutos
                            .SingleOrDefault(p => p.Produto_ID == idProduto);

                        if (produtoCompra != null)
                        {
                            var produtoGeral = await db.ProdutoEstoqueSemus
                                .SingleOrDefaultAsync(p => p.EstoqueSemus_ID == 1 && p.Produto_ID == produtoCompra.Produto_ID);

                            if (produtoGeral.QuantidadeEmFalta > produtoCompra.Quantidade)
                            {
                                produtoGeral.QuantidadeEmFalta -= produtoCompra.Quantidade;
                            }
                            else
                            {
                                produtoGeral.QuantidadeEmFalta = 0;
                            }

                            produtoGeral.QuantidadeTotal += produtoCompra.Quantidade;
                            produtoGeral.QuantidadeEntrada = produtoCompra.Quantidade;
                            produtoGeral.DataEntrada = DateTime.Now;
                            produtoGeral.UserID = userID;
                            db.Entry(produtoGeral).State = EntityState.Modified;
                        }
                    }
                }

                if (notProdutos != null)
                {
                    foreach (var item in notProdutos)
                    {
                        int idProduto = int.Parse(item);
                        var produtoCompra = pedidoCompra.PedidoCompraNaoCadastrados
                            .SingleOrDefault(p => p.Produto_ID == idProduto);

                        if (produtoCompra != null)
                        {
                            int novoProdutoID = MetodosGlobais.ExecutarUpdateEstoque.CadastrarProduto(produtoCompra);

                            var novoProdutoEstoque = new ProdutoEstoqueSemus
                            {
                                Produto_ID = novoProdutoID,
                                EstoqueSemus_ID = 1,
                                QuantidadeTotal = produtoCompra.Quantidade,
                                QuantidadeEntrada = produtoCompra.Quantidade,
                                DataEntrada = DateTime.Now,
                                UserID = userID
                            };

                            _ = db.ProdutoEstoqueSemus.Add(novoProdutoEstoque);
                            MetodosGlobais.ExecutarUpdateEstoque.InsertDeletePedidoProduto(idProduto, novoProdutoID);
                        }
                    }
                }

                pedidoCompra.Faturado = true;
                pedidoCompra.DataFaturado = DateTime.Now;
                pedidoCompra.UserID = userID;

                db.Entry(pedidoCompra).State = EntityState.Modified;
                _ = await db.SaveChangesAsync();

                TempData["Message"] = "Nova Compra faturada com sucesso.";
                return RedirectToAction("Index", new { tipoProduto = pedidoCompra.TipoProduto });
            }
            catch (Exception ex)
            {
                return ex.InnerException == null
                    ? RedirectToAction("Erro", "Home", new { Erro = ex.Message })
                    : RedirectToAction("Erro", "Home", new { Erro = ex.InnerException.Message });
            }
        }

        // Futuras Implementações
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoCompra pedidoCompra = await db.PedidoCompras.FindAsync(id);
            if (pedidoCompra == null)
            {
                return HttpNotFound();
            }
            return View(pedidoCompra);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoCompra pedidoCompra = await db.PedidoCompras.FindAsync(id);
            if (pedidoCompra == null)
            {
                return HttpNotFound();
            }
            return View(pedidoCompra);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,EstoqueSemus_ID,TipoProduto,Faturado,DataEntrada,DataFaturado")] PedidoCompra pedidoCompra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pedidoCompra).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(pedidoCompra);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PedidoCompra pedidoCompra = await db.PedidoCompras.FindAsync(id);
            if (pedidoCompra == null)
            {
                return HttpNotFound();
            }
            return View(pedidoCompra);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PedidoCompra pedidoCompra = await db.PedidoCompras.FindAsync(id);
            db.PedidoCompras.Remove(pedidoCompra);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
