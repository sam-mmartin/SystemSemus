using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using SistemaSemus.DAL;
using SistemaSemus.Models;

namespace SistemaSemus.Controllers
{
    public class PacienteController : Controller
    {
        #region Var e Contructor
        private SemusContext _dbContext;

        public PacienteController()
        {
        }

        public PacienteController(SemusContext dbContext)
        {
            Db = dbContext;
        }

        public SemusContext Db
        {
            get => _dbContext ?? HttpContext.GetOwinContext().GetUserManager<SemusContext>();
            set => _dbContext = value;
        }
        #endregion

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageSize = 3;
            int pageNumber = page ?? 1;
            var pacientes = await Db.Pacientes.ToListAsync();

            if (searchString == null)
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                pacientes = pacientes.Where(p => p.Nome.Contains(searchString)).ToList();
                if (pacientes.Count() == 0)
                {
                    pacientes = await Db.Pacientes.Where(p => p.ID.Contains(searchString)).ToListAsync();
                }
            }

            switch (sortOrder)
            {
                case "nome_desc":
                    pacientes = pacientes.OrderByDescending(p => p.Nome).ToList();
                    break;
                case "data_desc":
                    pacientes = pacientes.OrderByDescending(p => p.DataNascimento).ToList();
                    break;
                case "Data":
                    pacientes = pacientes.OrderBy(p => p.DataNascimento).ToList();
                    break;
                default:
                    pacientes = pacientes.OrderBy(p => p.Nome).ToList();
                    break;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeSortParm = string.IsNullOrEmpty(sortOrder) ? "nome_desc" : "";
            ViewBag.DataSortParm = sortOrder == "Data" ? "data_desc" : "Data";
            ViewBag.CurrentFilter = searchString;

            return View(pacientes.ToPagedList(pageNumber, pageSize));
        }

        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Paciente paciente = await Db.Pacientes.FindAsync(id);
                return paciente == null ? HttpNotFound() : (ActionResult)View(paciente);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Nome,RG,Orgao_Emissor,DataNascimento")] Paciente paciente)
        {
            try
            {
                if (MetodosGlobais.Validar.IsCpf(paciente.ID))
                {
                    paciente.ID = paciente.ID.Replace(".", "").Replace("-", "");
                }
                else
                {
                    TempData["Message"] = "Nº de CPF Inválido!";
                    return View(paciente);
                }

                if (ModelState.IsValid)
                {
                    _ = Db.Pacientes.Add(paciente);
                    _ = await Db.SaveChangesAsync();

                    TempData["Message"] = "Cadastro realizado com sucesso.";
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
            }

            return View(paciente);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Paciente paciente = await Db.Pacientes.FindAsync(id);
                return paciente == null ? HttpNotFound() : (ActionResult)View(paciente);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                id = id.Replace(".", "").Replace("-", "").Replace(" ", "");
                Paciente pacienteUpdate = await Db.Pacientes.FindAsync(id);
                string[] fieldsToBind = { "Nome", "RG", "Orgao_Emissor", "DataNascimento" };


                if (TryUpdateModel(pacienteUpdate, "", fieldsToBind))
                {
                    try
                    {
                        _ = await Db.SaveChangesAsync();

                        TempData["Message"] = "Atualização realizada com sucesso.";
                        return RedirectToAction("Index");
                    }
                    catch (RetryLimitExceededException)
                    {
                        ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                            "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
                    }
                }
                return View(pacienteUpdate);
            }
        }

        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Paciente paciente = db.Pacientes.Find(id);
        //    if (paciente == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(paciente);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Paciente paciente = db.Pacientes.Find(id);
        //    db.Pacientes.Remove(paciente);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && _dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }

            base.Dispose(disposing);
        }
    }
}
