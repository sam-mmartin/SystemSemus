using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class PedidoCompra
    {
        public int ID { get; set; }
        [ForeignKey("EstoqueSemus")]
        public int EstoqueSemus_ID { get; set; }
        public byte TipoProduto { get; set; }
        public bool Faturado { get; set; }
        [Display(Name = "Data de Entrada"), DataType(DataType.Date)]
        public DateTime DataEntrada { get; set; }
        [Display(Name = "Data do Faturamento"), DataType(DataType.Date)]
        public DateTime? DataFaturado { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        // Navegação
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual EstoqueSemus EstoqueSemus { get; set; }
        public virtual ICollection<PedidoCompraProduto> PedidoCompraProdutos { get; set; }
        public virtual ICollection<PedidoCompraNaoCadastrado> PedidoCompraNaoCadastrados { get; set; }
    }
}