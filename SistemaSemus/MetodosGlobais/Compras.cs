using SistemaSemus.DAL;
using SistemaSemus.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SistemaSemus.MetodosGlobais
{
    public class Compras
    {
        public static async Task<int> NovaCompra(byte tipoProduto)
        {
            using (SemusContext db = new SemusContext())
            {
                var pedidoCompra = new PedidoCompra
                {
                    EstoqueSemus_ID = 1,
                    TipoProduto = tipoProduto,
                    Faturado = false,
                    DataEntrada = DateTime.Now
                };

                _ = db.PedidoCompras.Add(pedidoCompra);
                _ = await db.SaveChangesAsync();
                return pedidoCompra.ID;
            }
        }

        public static async Task<int> NovoProdutoNaoCadastrado([Bind(Include = "TipoProduto,Quantidade,Descricao,DataEntrada")] ProdutoNaoCadastrado produtoNaoCadastrado)
        {
            using (SemusContext db = new SemusContext())
            {
                _ = db.ProdutoNaoCadatrados.Add(produtoNaoCadastrado);
                _ = await db.SaveChangesAsync();
                return produtoNaoCadastrado.ID;
            }
        }
    }
}