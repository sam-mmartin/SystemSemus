using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class Receitado
    {
        [Key]
        [ForeignKey("Produto")]
        public int ProdutoID { get; set; }
        public int ReceitaID { get; set; }
        public int Quantidade { get; set; }

        public virtual Produto Produto { get; set; }
        public virtual Receita Receita { get; set; }
    }
}