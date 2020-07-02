using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class ProdutoEstoqueSemus
    {
        // Chaves
        [ForeignKey("Produto")]
        public int Produto_ID { get; set; }
        [ForeignKey("EstoqueSemus")]
        public int EstoqueSemus_ID { get; set; }

        // Propriedades
        [Display(Name = "Quantidade")]
        public int QuantidadeTotal { get; set; }
        [Display(Name = "Última Entrada")]
        public int QuantidadeEntrada { get; set; }
        [Display(Name = "Última Saída")]
        public int QuantidadeSaida { get; set; }
        public int QuantidadeEmFalta { get; set; }
        [Display(Name = "Data de Entrada"), DataType(DataType.Date)]
        public DateTime DataEntrada { get; set; }
        [Display(Name = "Data de Saida"), DataType(DataType.Date)]
        public DateTime? DataSaida { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        // Navegação
        public virtual Produto Produto { get; set; }
        public virtual EstoqueSemus EstoqueSemus { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}