using System;
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
    public class MedicoController : Controller
    {
        #region Var e Contructor
        private SemusContext _dbContext;

        public MedicoController()
        {
        }

        public MedicoController(SemusContext dbContext)
        {
            Db = dbContext;
        }

        public SemusContext Db
        {
            get => _dbContext ?? HttpContext.GetOwinContext().GetUserManager<SemusContext>();
            set => _dbContext = value;
        }
        #endregion

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            var medicos = from m in Db.Medicos select m;

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
                medicos = medicos.Where(m => m.Nome.Contains(searchString));
                if (medicos.Count() == 0)
                {
                    medicos = Db.Medicos.Where(m => m.ID.Contains(searchString));
                    if (medicos.Count() == 0)
                    {
                        medicos = Db.Medicos.Where(m => m.Area_Atuacao.Contains(searchString));
                    }
                }
            }

            switch (sortOrder)
            {
                case "nome_desc":
                    medicos = medicos.OrderByDescending(m => m.Nome);
                    break;
                case "crm_desc":
                    medicos = medicos.OrderByDescending(m => m.ID);
                    break;
                case "area_desc":
                    medicos = medicos.OrderByDescending(m => m.Area_Atuacao);
                    break;
                case "Crm":
                    medicos = medicos.OrderBy(m => m.ID);
                    break;
                case "Area":
                    medicos = medicos.OrderBy(m => m.Area_Atuacao);
                    break;
                default:
                    medicos = medicos.OrderBy(m => m.Nome);
                    break;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeSortParm = string.IsNullOrEmpty(sortOrder) ? "nome_desc" : "";
            ViewBag.CRMSortParm = sortOrder == "Crm" ? "crm_desc" : "Crm";
            ViewBag.AreaSortParm = sortOrder == "Area" ? "area_desc" : "Area";
            ViewBag.CurrentFilter = searchString;

            return View(medicos.ToPagedList(pageNumber, pageSize));
        }

        public async Task<JsonResult> DetailsAsync(string id)
        {
            var medico = await Db.Medicos.FindAsync(id);
            var resultado = new
            {
                medico.ID,
                medico.Nome,
                Area = medico.Area_Atuacao,
                Cadastro = medico.DataCadastro.ToShortDateString(),
                Receitas = medico.Receitas.Count
            };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Nome,Area_Atuacao")] Medico medico)
        {
            try
            {
                Medico chaveduplicada = await Db.Medicos.FindAsync(medico.ID);

                if (chaveduplicada == null)
                {
                    if (ModelState.IsValid)
                    {
                        medico.DataCadastro = DateTime.Now;

                        _ = Db.Medicos.Add(medico);
                        _ = await Db.SaveChangesAsync();

                        TempData["Message"] = "Cadastro realizado com sucesso!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["Message"] = "Violação da restrição de Chave Primária! Este CRM já está cadastrado!";
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
            }

            return View(medico);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Medico medico = await Db.Medicos.FindAsync(id);
                return medico == null ? HttpNotFound() : (ActionResult)View(medico);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Nome", "Area_Atuacao", "RowVersion" };

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Medico medicoUpdate = await Db.Medicos.FindAsync(id);

                if (medicoUpdate == null)
                {
                    TempData["Message"] = "Não foi possível salvar as alterações. O registro foi apagado por outro usuário.";
                    return RedirectToAction("Index");
                }

                if (TryUpdateModel(medicoUpdate, fieldsToBind))
                {
                    try
                    {
                        Db.Entry(medicoUpdate).OriginalValues["RowVersion"] = rowVersion;
                        _ = await Db.SaveChangesAsync();

                        TempData["Message"] = "Atualização realizada com sucesso!";
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var entry = ex.Entries.Single();
                        var clientValues = (Medico)entry.Entity;
                        var databaseEntry = entry.GetDatabaseValues();

                        if (databaseEntry == null)
                        {
                            TempData["Message"] = "Não foi possível salvar as alterações. O registro foi apagado por outro usuário.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            var databaseValues = (Medico)databaseEntry.ToObject();

                            if (databaseValues.Nome != clientValues.Nome)
                                ModelState.AddModelError("Nome", "Registro Atual: " + databaseValues.Nome);

                            if (databaseValues.Area_Atuacao != clientValues.Area_Atuacao)
                                ModelState.AddModelError("Area_Atuacao", "Registro Atual: " + databaseValues.Area_Atuacao);

                            ModelState.AddModelError(string.Empty, "O registro que você tentou atualizar foi modificado por outro usuário após a obtenção do valor original. " +
                               "A operação de edição foi cancelada e os valores atuais no banco de dados foram exibidos. " +
                               "Se você ainda deseja editar este registro, clique no botão Salvar novamente. " +
                               "Caso contrário, clique no botão Voltar.");

                            medicoUpdate.RowVersion = databaseValues.RowVersion;
                        }
                    }
                    catch (RetryLimitExceededException)
                    {
                        ModelState.AddModelError("", "Não foi possível salvar as alterações. " +
                            "Tente novamente e, se o problema persistir, consulte o administrador do sistema.");
                    }
                }

                return View(medicoUpdate);
            }
        }

        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Medico medico = await Db.Medicos.FindAsync(id);
                return medico == null ? HttpNotFound() : (ActionResult)View(medico);
            }
        }

        [HttpPost, ActionName("DeleteAsync")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(string id)
        {
            Medico medico = Db.Medicos.Find(id);
            _ = Db.Medicos.Remove(medico);
            _ = await Db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
