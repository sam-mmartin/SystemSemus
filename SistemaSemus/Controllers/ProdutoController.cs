using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using SistemaSemus.DAL;
using SistemaSemus.Models;

namespace SistemaSemus.Controllers
{
    public class ProdutoController : Controller
    {
        #region Var e Contructor
        private SemusContext _dbContext;

        public ProdutoController()
        {
        }

        public ProdutoController(SemusContext dbContext)
        {
            Db = dbContext;
        }

        public SemusContext Db
        {
            get => _dbContext ?? HttpContext.GetOwinContext().GetUserManager<SemusContext>();
            set => _dbContext = value;
        }
        #endregion

        public async Task<ActionResult> Index(int estoqueID, byte TipoProduto, string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var estoque = await Db.EstoqueSemus.FindAsync(estoqueID);
            var produtos = estoque.ProdutoEstoqueSemus.Where(p => p.Produto.TipoProduto == TipoProduto);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                produtos = produtos.Where(m => m.Produto.Descricao.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "descricao_desc":
                    produtos = produtos.OrderByDescending(m => m.Produto.Descricao);
                    break;
                case "Categoria":
                    produtos = produtos.OrderBy(m => m.Produto.Categoria);
                    break;
                case "categoria_desc":
                    produtos = produtos.OrderByDescending(m => m.Produto.Categoria);
                    break;
                default:
                    produtos = produtos.OrderBy(m => m.Produto.Descricao);
                    break;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescricaoSortParm = string.IsNullOrEmpty(sortOrder) ? "descricao_desc" : "";
            ViewBag.CategoriaSortParm = sortOrder == "Categoria" ? "categoria_desc" : "Categoria";
            ViewBag.CurrentFilter = searchString;
            ViewBag.Descricao = estoque.Descricao;
            ViewBag.Estoque = estoqueID;
            ViewBag.TipoProduto = TipoProduto;

            return View(produtos.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> Details(byte tipoProduto, string searchString)
        {
            var produtos = await Db.Produtos.Where(p => p.TipoProduto == tipoProduto).ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                produtos = produtos.Where(p => p.Descricao.Contains(searchString)).ToList();
            }

            ViewBag.TipoProduto = tipoProduto;
            return View(produtos);
        }

        public ActionResult Create(byte TipoProduto)
        {
            var novoProduto = new Produto
            {
                TipoProduto = TipoProduto
            };

            ViewBag.Estoque = 1;
            return View(novoProduto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TipoProduto,Descricao,Categoria")] Produto produto, int Quantidade)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    produto.Descricao = produto.Descricao.ToUpper();
                    _ = Db.Produtos.Add(produto);

                    var novoEstoqueProduto = new ProdutoEstoqueSemus
                    {
                        Produto_ID = produto.ID,
                        EstoqueSemus_ID = 1,
                        QuantidadeTotal = Quantidade,
                        QuantidadeEntrada = Quantidade,
                        QuantidadeSaida = 0,
                        QuantidadeEmFalta = 0,
                        DataEntrada = DateTime.Now,
                        UserID = User.Identity.GetUserId()
                    };

                    _ = Db.ProdutoEstoqueSemus.Add(novoEstoqueProduto);
                    _ = await Db.SaveChangesAsync();

                    TempData["Message"] = "Cadastro efetuado com sucesso!";
                    return RedirectToAction("Index", new { estoqueID = 1, produto.TipoProduto });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
            }

            return View(produto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Estoque = 1;
            var produto = await Db.Produtos.FindAsync(id);
            return produto == null ? HttpNotFound() : (ActionResult)View(produto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, int EntradaEstoque)
        {
            string[] fieldsToBind = new string[] { "Descricao", "TipoProduto", "Categoria" };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var produtoUpdate = await Db.Produtos.FindAsync(id);

            if (produtoUpdate == null)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. O registro foi apagado por outro usuário.";
                return RedirectToAction("Index", (estoqueID: 1, produtoUpdate.TipoProduto));
            }

            if (TryUpdateModel(produtoUpdate, fieldsToBind))
            {
                try
                {
                    produtoUpdate.EstoqueSemus.SingleOrDefault(
                        p => p.EstoqueSemus_ID == 1 && p.Produto_ID == produtoUpdate.ID).QuantidadeEntrada = EntradaEstoque;
                    produtoUpdate.EstoqueSemus.SingleOrDefault(
                        p => p.EstoqueSemus_ID == 1 && p.Produto_ID == produtoUpdate.ID).QuantidadeTotal += EntradaEstoque;
                    produtoUpdate.EstoqueSemus.SingleOrDefault(
                        p => p.EstoqueSemus_ID == 1 && p.Produto_ID == produtoUpdate.ID).DataEntrada = DateTime.Now;
                    produtoUpdate.EstoqueSemus.SingleOrDefault(
                        p => p.EstoqueSemus_ID == 1 && p.Produto_ID == produtoUpdate.ID).UserID = User.Identity.GetUserId();

                    Db.Entry(produtoUpdate).State = EntityState.Modified;
                    _ = await Db.SaveChangesAsync();

                    TempData["Message"] = "Atualização realizada com sucesso!";
                    return RedirectToAction("Index", new { estoqueID = 1, produtoUpdate.TipoProduto });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                        "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
                }
            }

            return View(produtoUpdate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
