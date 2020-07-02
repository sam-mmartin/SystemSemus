using SistemaSemus.DAL;
using SistemaSemus.Models;
using SistemaSemus.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaSemus.MetodosGlobais
{
    public class ExecutarUpdateEstoque
    {
        public static async Task<BooleanCompra> VerificarIncrementar(ProdutoEstoqueSemus produtoGeral, PedidoProduto itemPedido, string userID)
        {
            using (SemusContext db = new SemusContext())
            {
                BooleanCompra retorno = new BooleanCompra
                {
                    Verifica = false,
                    Quantidade = 0
                };
                switch (produtoGeral.QuantidadeTotal)
                {
                    case 0:
                        produtoGeral.QuantidadeEmFalta += itemPedido.Quantidade;
                        produtoGeral.UserID = userID;

                        db.Entry(produtoGeral).State = EntityState.Modified;

                        retorno.Verifica = true;
                        retorno.Quantidade = itemPedido.Quantidade;
                        break;
                    default:
                        if (produtoGeral.QuantidadeTotal >= itemPedido.Quantidade)
                        {
                            var itemEstoque = itemPedido
                                .PedidoEstoque.EstoqueSemus.ProdutoEstoqueSemus
                                .SingleOrDefault(p => p.Produto_ID == produtoGeral.Produto_ID);

                            switch (itemEstoque)
                            {
                                case null:
                                    {
                                        // Adiciona o Produto no estoque da Unidade
                                        var novoItemEstoque = new ProdutoEstoqueSemus
                                        {
                                            Produto_ID = produtoGeral.Produto_ID,
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
                            var itemEstoque = itemPedido
                                .PedidoEstoque.EstoqueSemus.ProdutoEstoqueSemus
                                .SingleOrDefault(p => p.Produto_ID == produtoGeral.Produto_ID);

                            switch (itemEstoque)
                            {
                                case null:
                                    {
                                        // Adiciona o Produto no estoque da Unidade
                                        var novoItemEstoque = new ProdutoEstoqueSemus
                                        {
                                            Produto_ID = produtoGeral.Produto_ID,
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
                            retorno.Verifica = true;
                            retorno.Quantidade = itemPedido.Quantidade;
                        }

                        break;
                }

                _ = await db.SaveChangesAsync();
                return retorno;
            }
        }

        public static async Task<BooleanCompra> UpdateNaoCadastrado(PedidoProduto itemPedido)
        {
            using (SemusContext db = new SemusContext())
            {
                BooleanCompra retorno = new BooleanCompra
                {
                    Verifica = false,
                    Quantidade = 0
                };

                var produtoNaoCadastrado = await db.ProdutoNaoCadatrados.SingleOrDefaultAsync(
                    p => p.Descricao.Equals(itemPedido.Descricao));

                switch (produtoNaoCadastrado)
                {
                    case null:
                        {
                            var novoProdutoSemEstoque = new ProdutoNaoCadastrado
                            {
                                TipoProduto = itemPedido.PedidoEstoque.TipoPedido,
                                Quantidade = itemPedido.Quantidade,
                                Descricao = itemPedido.Descricao,
                                DataEntrada = DateTime.Now,
                                DataPedido = DateTime.Now
                            };

                            retorno.Quantidade = await Compras.NovoProdutoNaoCadastrado(novoProdutoSemEstoque);
                            retorno.Verifica = true;
                            break;
                        }

                    default:
                        produtoNaoCadastrado.Quantidade += itemPedido.Quantidade;
                        produtoNaoCadastrado.DataPedido = DateTime.Now;
                        db.Entry(produtoNaoCadastrado).State = EntityState.Modified;
                        retorno.Quantidade = produtoNaoCadastrado.ID;
                        retorno.Verifica = true;
                        break;
                }

                _ = await db.SaveChangesAsync();
                return retorno;
            }
        }

        public static int CadastrarProduto(PedidoCompraNaoCadastrado produto)
        {
            using (SemusContext db = new SemusContext())
            {
                var novoProduto = new Produto
                {
                    Descricao = produto.ProdutoNaoCadastrado.Descricao,
                    TipoProduto = produto.ProdutoNaoCadastrado.TipoProduto
                };
                _ = db.Produtos.Add(novoProduto);
                _ = db.SaveChanges();
                return novoProduto.ID;
            }
        }

        public static void InsertDeletePedidoProduto(int idProduto, int produtoID)
        {
            using (SemusContext db = new SemusContext())
            {
                var produtoNaoCadastrado = db.PedidoCompraNaoCadastrados.Where(p => p.Produto_ID == idProduto).ToList();
                var produto = db.ProdutoNaoCadatrados.Find(idProduto);
                var listNaoCadastrados = new List<PedidoCompraNaoCadastrado>();

                foreach (var item in produtoNaoCadastrado)
                {
                    var novoProdutoPedido = new PedidoCompraProduto
                    {
                        Produto_ID = produtoID,
                        PedidoCompra_ID = item.PedidoCompra_ID,
                        Quantidade = item.Quantidade
                    };
                    _ = db.PedidoCompraProdutos.Add(novoProdutoPedido);
                    listNaoCadastrados.Add(item);
                }

                foreach (var item in listNaoCadastrados)
                {
                    _ = db.PedidoCompraNaoCadastrados.Remove(item);
                }

                _ = db.ProdutoNaoCadatrados.Remove(produto);
                _ = db.SaveChanges();
            }
        }
    }
}