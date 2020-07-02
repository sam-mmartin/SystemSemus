using SistemaSemus.DAL;
using SistemaSemus.Models;
using System.Linq;
using System.Web.Mvc;

namespace SistemaSemus.Controllers
{
    public class VerificarDisponibilidadeController : Controller
    {
        protected readonly SemusContext db = new SemusContext();
        public JsonResult VerificaQuantidade(byte tipoProduto, int produtoID, int quantidade)
        {
            ProdutoEstoqueSemus produto = db.EstoqueSemus
                .SingleOrDefault(e => e.ID == 1)
                .ProdutoEstoqueSemus
                .SingleOrDefault(p => p.Produto.TipoProduto == tipoProduto && p.Produto_ID == produtoID);

            if (quantidade <= produto.QuantidadeTotal || produto.QuantidadeTotal == 0)
            {
                var resultado = new
                {
                    Permitir = true
                };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var resultado = new
                {
                    Permitir = false,
                    Desc_Produto = produto.Produto.Descricao,
                    Quant_Produto = produto.QuantidadeTotal
                };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CarrinhoReceita(int produtoID, int quantidade, int estoqueID)
        {
            ProdutoEstoqueSemus produto = db.EstoqueSemus
                .SingleOrDefault(e => e.ID == estoqueID)
                .ProdutoEstoqueSemus
                .SingleOrDefault(p => p.Produto_ID == produtoID && p.Produto.TipoProduto == 1);

            if (quantidade <= produto.QuantidadeTotal || produto.QuantidadeTotal == 0)
            {
                var resultado = new
                {
                    Permitir = true
                };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var resultado = new
                {
                    Permitir = false,
                    Desc_Produto = produto.Produto.Descricao,
                    Quant_Produto = produto.QuantidadeTotal
                };

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }
    }
}