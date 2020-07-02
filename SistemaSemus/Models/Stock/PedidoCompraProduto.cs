using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class PedidoCompraProduto
    {
        [ForeignKey("Produto")]
        public int Produto_ID { get; set; }
        [ForeignKey("PedidoCompra")]
        public int PedidoCompra_ID { get; set; }
        public int Quantidade { get; set; }

        public virtual Produto Produto { get; set; }
        public virtual PedidoCompra PedidoCompra { get; set; }
    }
}