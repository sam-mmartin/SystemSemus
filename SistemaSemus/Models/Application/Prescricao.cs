using SistemaSemus.Models.Application;

namespace SistemaSemus.Models
{
    public class Prescricao
    {
        public int ID { get; set; }
        public int? ProdutoID { get; set; }
        public int? ProdutoSemEstoqueID { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }

        public virtual ReceitaMedica ReceitaMedica { get; set; }
    }
}