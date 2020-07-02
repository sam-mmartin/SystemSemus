using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SistemaSemus.DAL;
using SistemaSemus.Models;
using System.Data.Entity.Infrastructure;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using SistemaSemus.Filters;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;

namespace SistemaSemus.Controllers
{
    public class EstoqueSemusController : Controller
    {
        #region Var e Contructor
        private SemusContext _dbContext;

        public EstoqueSemusController()
        {
        }

        public EstoqueSemusController(SemusContext dbContext)
        {
            Db = dbContext;
        }

        public SemusContext Db
        {
            get => _dbContext ?? HttpContext.GetOwinContext().GetUserManager<SemusContext>();
            set => _dbContext = value;
        }
        #endregion

        public async Task<ActionResult> Index(string searchString)
        {
            var stock = await Db.EstoqueSemus.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                stock = stock.Where(e => e.Descricao.Contains(searchString)).ToList();
            }
            return View(stock);
        }

        [ClaimsAuthorize("AdminStock", "true")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (id == 1)
            {
                ViewBag.Pedidos = await Db.PedidoEstoques.Where(p => p.Faturado == false).CountAsync();
            }

            ViewBag.Estoque = id;
            var estoqueSemus = await Db.EstoqueSemus.FindAsync(id);
            return estoqueSemus == null ? HttpNotFound() : (ActionResult)View(estoqueSemus);
        }

        [ClaimsAuthorize("AdminStock", "true")]
        public ActionResult Create()
        {
            return View();
        }

        [ClaimsAuthorize("AdminStock", "true")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Descricao")] EstoqueSemus estoqueSemus)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    estoqueSemus.UserID = User.Identity.GetUserId();
                    estoqueSemus.DataCadastro = DateTime.Now;
                    estoqueSemus.DataAtualizacao = DateTime.Now;

                    _ = Db.EstoqueSemus.Add(estoqueSemus);
                    _ = await Db.SaveChangesAsync();
                    TempData["Message"] = "Novo Estoque criado com sucesso.";
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
            }
            return View(estoqueSemus);
        }

        [ClaimsAuthorize("AdminStock", "true")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var estoqueSemus = await Db.EstoqueSemus.FindAsync(id);
            return estoqueSemus == null ? HttpNotFound() : (ActionResult)View(estoqueSemus);
        }

        [ClaimsAuthorize("AdminStock", "true")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? ID, EstoqueSemus estoqueSemus)
        {
            string[] fieldsToBind = new string[] { "Descricao" };
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var stockUpdate = await Db.EstoqueSemus.FindAsync(ID);
            if (stockUpdate == null)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. O registro foi apagado por outro usuário.";
                return RedirectToAction("Index");
            }

            if (TryUpdateModel(stockUpdate, fieldsToBind))
            {
                try
                {
                    stockUpdate.DataAtualizacao = DateTime.Now;
                    stockUpdate.UserID = User.Identity.GetUserId();
                    Db.Entry(stockUpdate).State = EntityState.Modified;
                    await Db.SaveChangesAsync();

                    TempData["Message"] = "Atualização realizada com sucesso!";
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                        "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
                }
            }
            return View(estoqueSemus);
        }

        // Actions Para Implementação Futura
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var estoqueSemus = await db.EstoqueSemus.FindAsync(id);
        //    if (estoqueSemus == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(estoqueSemus);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    var estoqueSemus = await db.EstoqueSemus.FindAsync(id);
        //    db.EstoqueSemus.Remove(estoqueSemus);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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
