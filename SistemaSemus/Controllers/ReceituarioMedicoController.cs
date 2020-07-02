using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SistemaSemus.DAL;
using SistemaSemus.Models;
using SistemaSemus.Models.Application;
using SistemaSemus.ViewModels.Application;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SistemaSemus.Controllers
{
    public class ReceituarioMedicoController : Controller
    {
        #region Var e Contructor
        private SemusContext _dbContext;
        private ApplicationUserManager _userManager;

        public ReceituarioMedicoController()
        {
        }

        public ReceituarioMedicoController(SemusContext dbContext, ApplicationUserManager userManager)
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

        public async Task<ActionResult> IndexAsync(string searchString)
        {
            ViewBag.NavBar = true;
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Replace(".", "").Replace("-", "");
                var paciente = await Db.Pacientes.FindAsync(searchString);
                return View(paciente);
            }
            return View();
        }

        public async Task<ActionResult> CreateAsync(string id)
        {
            var paciente = await Db.Pacientes.FindAsync(id);
            var doctor = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var doctorID = doctor.Claims.SingleOrDefault(c => c.ClaimType.Equals("Doctor")).ClaimValue;

            var novaReceita = new ReceitaViewModel
            {
                MedicoID = doctorID,
                PacienteID = Convert.ToUInt64(paciente.ID).ToString(@"000\.000\.000\-00"),
                NomeMedico = doctor.Nome,
                NomePaciente = paciente.Nome,
                UserID = User.Identity.GetUserId()
            };

            ViewBag.Medicamentos = new SelectList(await Db.ProdutoEstoqueSemus
                .Where(m => m.EstoqueSemus_ID == 1 && m.Produto.TipoProduto == 1)
                .Select(p => p.Produto)
                .ToListAsync(), "ID", "Descricao");

            ViewBag.ProdutosSemEstoque = Db.ProdutoNaoCadatrados.Count() == 0
                ? null
                : new SelectList(await Db.ProdutoNaoCadatrados.Where(m => m.TipoProduto == 1).ToListAsync(), "ID", "Descricao");
            return View(novaReceita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(ReceitaViewModel model, string[] produtos, string[] produtos2, string[] produtos3)
        {
            try
            {
                if (produtos == null && produtos2 == null && produtos3 == null)
                {
                    TempData["Message"] = "Nenhum medicamento receitado.";
                    return RedirectToAction("CreateAsync", new { id = model.PacienteID });
                }

                if (ModelState.IsValid)
                {
                    var novaReceita = new ReceitaMedica
                    {
                        MedicoID = model.MedicoID,
                        PacienteID = model.PacienteID.Replace(".", "").Replace("-", ""),
                        DataCadastro = DateTime.Now,
                        UserID = model.UserID
                    };

                    novaReceita.Prescricaos = new List<Prescricao>();

                    if (produtos != null)
                    {
                        foreach (var item in produtos)
                        {
                            string[] dados = item.Split('/');
                            int produtoID = int.Parse(dados[0]);
                            var produto = await Db.Produtos.FindAsync(produtoID);

                            var medicamento = new Prescricao
                            {
                                ProdutoID = produto.ID,
                                Descricao = produto.Descricao,
                                Quantidade = int.Parse(dados[1])
                            };

                            novaReceita.Prescricaos.Add(medicamento);
                        }
                    }

                    if (produtos2 != null)
                    {
                        foreach (var item in produtos2)
                        {
                            string[] dados = item.Split('|');
                            var medicamento = new Prescricao
                            {
                                Descricao = dados[0],
                                Quantidade = int.Parse(dados[1])
                            };
                            novaReceita.Prescricaos.Add(medicamento);
                        }
                    }

                    if (produtos3 != null)
                    {
                        foreach (var item in produtos3)
                        {
                            string[] dados = item.Split('-');
                            int produtoID = int.Parse(dados[0]);
                            var produto = await Db.ProdutoNaoCadatrados.FindAsync(produtoID);

                            var medicamento = new Prescricao
                            {
                                ProdutoSemEstoqueID = produto.ID,
                                Descricao = produto.Descricao,
                                Quantidade = int.Parse(dados[1])
                            };

                            novaReceita.Prescricaos.Add(medicamento);
                        }
                    }

                    _ = Db.ReceitaMedicas.Add(novaReceita);
                    _ = await Db.SaveChangesAsync();

                    TempData["Message"] = "Receita incluída no sistema.";
                    return RedirectToAction("IndexAsync", new { searchString = model.PacienteID });
                }
            }
            catch (RetryLimitExceededException)
            {
                TempData["Message"] = "Não foi possível salvar as alterações. " +
                    "Tente novamente e, se o problema persistir, consulte o administrador do sistema.";
            }

            return RedirectToAction("CreateAsync", new { id = model.PacienteID });
        }
    }
}