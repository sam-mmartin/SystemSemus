using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using SistemaSemus.DAL;
using SistemaSemus.Models;
using SistemaSemus.ViewModels.Application;
using SistemaSemus.ViewModels.Stock;

namespace SistemaSemus.Controllers
{
    public class ReceitaController : Controller
    {
        #region Var e Contructor
        private SemusContext _dbContext;
        private ApplicationUserManager _userManager;

        public ReceitaController()
        {
        }

        public ReceitaController(SemusContext dbContext, ApplicationUserManager userManager)
        {
            Db = dbContext;
            UserManager = userManager;
        }

        public SemusContext Db
        {
            get => _dbContext ?? HttpContext.GetOwinContext().GetUserManager<SemusContext>();
            set => _dbContext = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }
        #endregion

        public async Task<ActionResult> RedirectActionAsync()
        {
            var usuario = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            return usuario.Funcao.Descricao.Equals("Médico")
                ? RedirectToAction("IndexAsync", "ReceituarioMedico")
                : RedirectToAction("Index");
        }

        public async Task<ActionResult> Index(string Registers, string ID, string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (string.IsNullOrEmpty(Registers))
            {
                ViewBag.Type = "";
                return View();
            }
            else
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;
                int estoqueID = int.Parse(UserManager
                    .GetClaims(User.Identity.GetUserId())
                    .SingleOrDefault(c => c.Type == "Estoque")
                    .Value);

                if (searchString == null)
                {
                    searchString = currentFilter;
                }
                else
                {
                    page = 1;
                }

                ViewBag.ID = string.IsNullOrEmpty(ID) ? "" : ID;
                ViewBag.CurrentSort = sortOrder;
                ViewBag.DataSortParm = string.IsNullOrEmpty(sortOrder) ? "data_desc" : "";
                ViewBag.PacienteSortParm = sortOrder == "Paciente" ? "paciente_desc" : "Paciente";
                ViewBag.MedicoSortParm = sortOrder == "Medico" ? "medico_desc" : "Medico";
                ViewBag.CurrentFilter = searchString;
                ViewBag.NavBar = false;

                switch (Registers)
                {
                    // Retorna consulta somente registros relativos a paciente especifico
                    case "Paciente":
                        {
                            ID = ID.Replace(".", "").Replace("-", "").Replace(" ", "");
                            var receitas = await Db.Receitas
                                .Where(r => r.PacienteID.Equals(ID) && r.EstoqueSemusID == estoqueID)
                                .ToListAsync();

                            // Verifica se a string de busca está vazia
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                // Realiza a busca de registros pelo nome do médico
                                receitas = await Db.Receitas
                                    .Where(r => r.Medico.Nome.Contains(searchString) && r.EstoqueSemusID == estoqueID)
                                    .ToListAsync();

                                if (receitas.Count() == 0)
                                {
                                    // Realiza a busca de registros pelo CRM do médico
                                    receitas = await Db.Receitas
                                        .Where(r => r.Medico.ID.Contains(searchString) && r.EstoqueSemusID == estoqueID)
                                        .ToListAsync();
                                }
                            }

                            switch (sortOrder)
                            {
                                case "data_desc":
                                    receitas = receitas.OrderByDescending(r => r.DataCadastro).ToList();
                                    break;

                                case "medico_desc":
                                    receitas = receitas.OrderByDescending(r => r.Medico.Nome).ToList();
                                    break;

                                case "Medico":
                                    receitas = receitas.OrderBy(r => r.Medico.Nome).ToList();
                                    break;
                                default:
                                    receitas = receitas.OrderBy(r => r.DataCadastro).ToList();
                                    break;
                            }

                            ViewBag.Type = "Paciente";
                            return View(receitas.ToPagedList(pageNumber, pageSize));
                        }

                    case "Medico":
                        {
                            var receitas = await Db.Receitas
                                .Where(r => r.MedicoID.Equals(ID) && r.EstoqueSemusID == estoqueID)
                                .ToListAsync();

                            // Verifica se a string de busca está vazia
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                // Realiza a busca pelo nome de pacientes
                                receitas = receitas
                                    .Where(r => r.Paciente.Nome.Contains(searchString))
                                    .ToList();
                            }

                            switch (sortOrder)
                            {
                                case "data_desc":
                                    receitas = receitas.OrderByDescending(r => r.DataCadastro).ToList();
                                    break;
                                case "paciente_desc":
                                    receitas = receitas.OrderByDescending(r => r.Paciente.Nome).ToList();
                                    break;
                                case "Paciente":
                                    receitas = receitas.OrderBy(r => r.Paciente.Nome).ToList();
                                    break;
                                default:
                                    receitas = receitas.OrderBy(r => r.DataCadastro).ToList();
                                    break;
                            }

                            ViewBag.Type = "Medico";
                            return View(receitas.ToPagedList(pageNumber, pageSize));
                        }

                    default:
                        {
                            var receitas = await Db.Receitas.Where(r => r.EstoqueSemusID == estoqueID).ToListAsync();
                            // Realiza a consulta pela data informada.
                            if (!string.IsNullOrEmpty(ID))
                            {
                                DateTime buscar_data = DateTime.Parse(ID);
                                receitas = receitas
                                    .Where(r => DbFunctions.TruncateTime(r.DataCadastro) == buscar_data.Date && r.EstoqueSemusID == estoqueID)
                                    .ToList();
                            }

                            // Verifica se a string de busca está vazia
                            if (!string.IsNullOrEmpty(searchString))
                            {
                                // Realiza a busca pelo nome de pacientes
                                receitas = receitas
                                    .Where(r => r.Paciente.Nome.Contains(searchString))
                                    .ToList();

                                // Verifica se foram encontrados registros
                                if (receitas.Count() == 0)
                                {
                                    // Realiza a busca de registros pelo nome do médico
                                    receitas = await Db.Receitas
                                        .Where(r => r.Medico.Nome.Contains(searchString) && r.EstoqueSemusID == estoqueID)
                                        .ToListAsync();

                                    if (receitas.Count() == 0)
                                    {
                                        receitas = await Db.Receitas
                                            .Where(r => r.Medico.ID.Contains(searchString) && r.EstoqueSemusID == estoqueID)
                                            .ToListAsync();
                                    }
                                }
                            }

                            switch (sortOrder)
                            {
                                case "data_desc":
                                    receitas = receitas.OrderByDescending(r => r.DataCadastro).ToList();
                                    break;
                                case "paciente_desc":
                                    receitas = receitas.OrderByDescending(r => r.Paciente.Nome).ToList();
                                    break;
                                case "medico_desc":
                                    receitas = receitas.OrderByDescending(r => r.Medico.Nome).ToList();
                                    break;
                                case "Paciente":
                                    receitas = receitas.OrderBy(r => r.Paciente.Nome).ToList();
                                    break;
                                case "Medico":
                                    receitas = receitas.OrderBy(r => r.Medico.Nome).ToList();
                                    break;
                                default:
                                    receitas = receitas.OrderBy(r => r.DataCadastro).ToList();
                                    break;
                            }

                            ViewBag.Type = "All";
                            return View(receitas.ToPagedList(pageNumber, pageSize));
                        }
                }
            }
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Receita não encontrada. O registro foi apagado por outro usuário.";
                return RedirectToAction("Index");
            }
            else
            {
                Receita receita = await Db.Receitas.FindAsync(id);
                return receita == null ? HttpNotFound() : (ActionResult)View(receita);
            }
        }

        public async Task<ActionResult> Create(int id)
        {
            int estoqueID = int.Parse(UserManager
                    .GetClaims(User.Identity.GetUserId())
                    .SingleOrDefault(c => c.Type == "Estoque")
                    .Value);

            var receitaMedica = await Db.ReceitaMedicas.FindAsync(id);
            var entregaViewModel = new EntregaViewModel
            {
                ReceitaMedicaID = id,

                EntregaMedicamento = new EntregaMedicamentoViewModel
                {
                    EstoqueID = estoqueID,
                    MedicoID = receitaMedica.MedicoID,
                    PacienteID = receitaMedica.PacienteID,
                    NomeMedico = receitaMedica.Medico.Nome,
                    NomePaciente = receitaMedica.Paciente.Nome
                },

                DisponibilidadeProdutos = new List<DisponibilidadeProdutoViewModel>()
            };

            foreach (var item in receitaMedica.Prescricaos)
            {
                if (item.ProdutoID != null)
                {
                    var produto = await Db.Produtos.SingleOrDefaultAsync(p => p.ID == item.ProdutoID);

                    var addMedicamento = new MedicamentoViewModel
                    {
                        ProdutoID = item.ProdutoID,
                        Descricao = item.Descricao,
                        Quantidade = item.Quantidade
                    };
                    var produtoDisponivel = new DisponibilidadeProdutoViewModel
                    {
                        ID = produto.ID,
                        Descricao = produto.Descricao
                    };

                    foreach (var elemento in produto.EstoqueSemus)
                    {
                        var stockProduct = new StockProductQuantidadeViewModel
                        {
                            Stock = elemento.EstoqueSemus.Descricao,
                            Quantidade = elemento.QuantidadeTotal
                        };
                        produtoDisponivel.StockProductQuantidades.Add(stockProduct);
                    }

                    entregaViewModel.DisponibilidadeProdutos.Add(produtoDisponivel);
                    entregaViewModel.EntregaMedicamento.MedicamentoViewModels.Add(addMedicamento);
                }
            }
            return View(entregaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EntregaViewModel model, string[] medicamentos, int[] item_Quantidade)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var novaReceita = new Receita
                    {
                        EstoqueSemusID = model.EntregaMedicamento.EstoqueID,
                        MedicoID = model.EntregaMedicamento.MedicoID,
                        PacienteID = model.EntregaMedicamento.PacienteID,
                        UserID = User.Identity.GetUserId(),
                        Receitados = new List<Receitado>()
                    };

                    if (medicamentos != null)
                    {
                        for (int i = 0; i < medicamentos.Length; i++)
                        {
                            var receitado = new Receitado
                            {
                                ProdutoID = int.Parse(medicamentos[i]),
                                Quantidade = item_Quantidade[i]
                            };
                            novaReceita.Receitados.Add(receitado);
                        }
                    }

                    novaReceita.DataCadastro = DateTime.Now;
                    _ = Db.Receitas.Add(novaReceita);
                    _ = await Db.SaveChangesAsync();

                    TempData["Message"] = "Entrega finalizada com sucesso.";
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.";
            }

            return RedirectToAction("Create", new { id = model.ReceitaMedicaID });
        }

        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    else
        //    {
        //        Receita receita = db.Receitas.Find(id);
        //        if (receita == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        else
        //        {
        //            ViewBag.MedicoID = new SelectList(db.Medicos, "ID", "Nome", receita.MedicoID);
        //            ViewBag.PacienteID = new SelectList(db.Pacientes, "ID", "Nome", receita.PacienteID);
        //            return View(receita);
        //        }
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,DataCadastro,MedicoID,PacienteID")] Receita receita)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(receita).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MedicoID = new SelectList(db.Medicos, "ID", "Nome", receita.MedicoID);
        //    ViewBag.PacienteID = new SelectList(db.Pacientes, "ID", "Nome", receita.PacienteID);
        //    return View(receita);
        //}

        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Receita receita = db.Receitas.Find(id);
        //    if (receita == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(receita);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Receita receita = db.Receitas.Find(id);
        //    db.Receitas.Remove(receita);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public async Task<PartialViewResult> BuscarReceitaMedica(int tipoBusca, string id)
        {
            switch (tipoBusca)
            {
                case 1:
                    int intID = int.Parse(id);
                    var receita = await Db.ReceitaMedicas.Where(r => r.ID == intID)
                                                         .ToListAsync();
                    return PartialView(receita);
                case 2:
                    id = id.Replace(".", "").Replace("-", "");
                    var receitas = await Db.ReceitaMedicas.Where(r => r.PacienteID.Equals(id))
                                                          .ToListAsync();
                    return PartialView(receitas);
                default:
                    return PartialView();
            }
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
