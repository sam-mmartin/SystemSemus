using Microsoft.AspNet.Identity.Owin;
using SistemaSemus.DAL;
using SistemaSemus.Filters;
using SistemaSemus.Models;
using SistemaSemus.ViewModels;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaSemus.Controllers
{
    [ClaimsAuthorize("Developer", "true")]
    public class ClaimsController : Controller
    {
        #region Var & Constructor
        private ApplicationUserManager _userManager;
        private SemusContext _dbContext;

        public ClaimsController()
        {

        }

        public ClaimsController(ApplicationUserManager userManager, SemusContext dbContext)
        {
            UserManager = userManager;
            DBContext = dbContext;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                _userManager = value;
            }
        }

        public SemusContext DBContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().GetUserManager<SemusContext>();
            }

            private set
            {
                _dbContext = value;
            }
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            return View(await DBContext.Claims.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Descricao")] Claims model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _ = DBContext.Claims.Add(model);
                    _ = await DBContext.SaveChangesAsync();
                    TempData["Message"] = "Nova Claim criada com sucesso.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return ex.InnerException == null
                    ? RedirectToAction("Erro", "Home", new { Erro = ex.Message })
                    : RedirectToAction("Erro", "Home", new { Erro = ex.InnerException.Message });
            }
        }

        public async Task<ActionResult> SetUserClaimAsync(string id)
        {
            ViewBag.Type = new SelectList
                (
                    await DBContext.Claims.ToListAsync(),
                    "Descricao",
                    "Descricao"
                );

            ViewBag.User = await UserManager.FindByIdAsync(id);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetUserClaimAsync(ClaimViewModel model, string id)
        {
            try
            {
                _ = await UserManager.AddClaimAsync(id, new Claim(model.Type, model.Value));
            }
            catch (RetryLimitExceededException)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.";
            }

            return RedirectToAction("IndexAsync", "UsuarioManager", new { id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }

            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
    }
}