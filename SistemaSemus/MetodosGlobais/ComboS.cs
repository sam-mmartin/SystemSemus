using SistemaSemus.DAL;
using SistemaSemus.Listas;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SistemaSemus.MetodosGlobais
{
    public class ComboS
    {
        public static SelectList TipoProduto { get; private set; }
        public static SelectList TipoBuscaReceita { get; private set; }
        public static SelectList Setor { get; set; }
        public static SelectList Funcao { get; set; }
        public static SelectList Medico { get; set; }
        public static SelectList Paciente { get; set; }

        public static void Combo()
        {
            TipoProduto = new SelectList(new TipoProduto().ListaTipoProduto(), "ID", "Descricao");
            TipoBuscaReceita = new SelectList(new TipoBuscaReceita().ListaTipoBuscaReceita(), "ID", "Descricao");
        }

        public static async Task ComboAsync()
        {
            using (SemusContext db = new SemusContext())
            {
                Setor = new SelectList(await db.Setors.OrderBy(x => x.Descricao).ToListAsync(), "ID", "Descricao");
                Funcao = new SelectList(await db.Funcaos.OrderBy(x => x.Descricao).ToListAsync(), "ID", "Descricao");
                Medico = new SelectList(await db.Medicos.OrderBy(x => x.Nome).ToListAsync(), "ID", "Nome");
                Paciente = new SelectList(await db.Pacientes.OrderBy(x => x.Nome).ToListAsync(), "ID", "Nome");
            }
        }
    }
}