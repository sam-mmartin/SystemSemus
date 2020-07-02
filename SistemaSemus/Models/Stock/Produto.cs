using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaSemus.Models
{
    public class Produto
    {
        // Chave
        public int ID { get; set; }

        // Propriedades
        public byte TipoProduto { get; set; }
        [Display(Name = "Descrição"), StringLength(80)]
        public string Descricao { get; set; }
        [StringLength(20)]
        public string Categoria { get; set; }

        // Navegação
        public virtual ICollection<ProdutoEstoqueSemus> EstoqueSemus { get; set; }
    }
}