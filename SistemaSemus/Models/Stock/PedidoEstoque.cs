using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class PedidoEstoque
    {
        public int ID { get; set; }
        public int EstoqueSemusID { get; set; }
        public byte TipoPedido { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataFaturado { get; set; }
        public bool Faturado { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        // Navegação
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual EstoqueSemus EstoqueSemus { get; set; }
        public virtual ICollection<PedidoProduto> PedidoProdutos { get; set; }
    }
}