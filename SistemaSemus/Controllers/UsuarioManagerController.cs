using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SistemaSemus.DAL;
using SistemaSemus.Filters;
using SistemaSemus.Models;
using SistemaSemus.Models.Client;
using SistemaSemus.ViewModels;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaSemus.Controllers
{
    [ClaimsAuthorize("SuperAdmin", "true")]
    public class UsuarioManagerController : Controller
    {
        #region Var e Contructor
        private ApplicationUserManager _userManager;
        private SemusContext _dbContext;

        public UsuarioManagerController()
        {
        }

        public UsuarioManagerController(SemusContext dbContext, ApplicationUserManager userManager)
        {
            DbContext = dbContext;
            UserManager = userManager;
        }

        public SemusContext DbContext
        {
            get { return _dbContext ?? HttpContext.GetOwinContext().GetUserManager<SemusContext>(); }
            set { _dbContext = value; }
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
        #endregion

        #region Indexs
        public async Task<ActionResult> IndexAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.User = id;
            }
            return View(await UserManager.Users.ToListAsync());
        }

        public async Task<ActionResult> IndexSetor()
        {
            return View(await DbContext.Setors.ToListAsync());
        }

        public async Task<ActionResult> IndexFuncao()
        {
            return View(await DbContext.Funcaos.ToListAsync());
        }
        #endregion

        #region Create
        public async Task<ActionResult> RegisterAsync()
        {
            await MetodosGlobais.ComboS.ComboAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = new ApplicationUser
                {
                    Nome = model.Nome,
                    UserName = model.UserName,
                    Email = model.Email,
                    Endereco = model.Endereco,
                    Nascimento = model.Nascimento,
                    PhoneNumber = model.Fone,
                    FuncaoID = model.Funcao,
                    SetorID = model.Setor
                };

                IdentityResult result = await UserManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Usuario registrado com sucesso";
                    return RedirectToAction("IndexAsync");
                }
                AddErrors(result);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSetor([Bind(Include = "Descricao")] Setor model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _ = DbContext.Setors.Add(model);
                    _ = await DbContext.SaveChangesAsync();
                    TempData["Message"] = "Novo Setor criado com sucesso.";
                    return RedirectToAction("IndexSetor");
                }
            }
            catch (RetryLimitExceededException)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.";
            }
            return RedirectToAction("IndexSetor");
        }

        [HttpPost]
        public async Task<ActionResult> CreateFuncao([Bind(Include = "Descricao")] Funcao model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _ = DbContext.Funcaos.Add(model);
                    _ = await DbContext.SaveChangesAsync();
                    TempData["Message"] = "Nova função criado com sucesso.";
                    return RedirectToAction("IndexFuncao");
                }
            }
            catch (RetryLimitExceededException)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.";
            }
            return RedirectToAction("IndexFuncao");
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Auxiliares
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion
    }
}