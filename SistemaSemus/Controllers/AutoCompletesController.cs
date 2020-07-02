using SistemaSemus.DAL;
using System.Linq;
using System.Web.Mvc;

namespace SistemaSemus.Controllers
{
    public class AutoCompletesController : Controller
    {
        protected readonly SemusContext db = new SemusContext();
        public ActionResult AutoCompleteNomePaciente(string nome)
        {
            return Json(db.Pacientes.Where(
                n => n.Nome.StartsWith(nome)).Select(s => s.Nome).Take(10).ToList(),
                JsonRequestBehavior.AllowGet);
        }
    }
}