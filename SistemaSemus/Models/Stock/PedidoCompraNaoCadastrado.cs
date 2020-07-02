using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class PedidoCompraNaoCadastrado
    {
        [ForeignKey("ProdutoNaoCadastrado")]
        public int Produto_ID { get; set; }
        [ForeignKey("PedidoCompra")]
        public int PedidoCompra_ID { get; set; }
        public int Quantidade { get; set; }

        public virtual ProdutoNaoCadastrado ProdutoNaoCadastrado { get; set; }
        public virtual PedidoCompra PedidoCompra { get; set; }
    }
}