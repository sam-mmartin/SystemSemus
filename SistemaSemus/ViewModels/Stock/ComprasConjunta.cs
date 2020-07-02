using SistemaSemus.Models;
using System.Collections.Generic;

namespace SistemaSemus.ViewModels
{
    public class ComprasConjunta
    {
        public int PedidoCompraID { get; set; }
        public PedidoCompra PedidoCompra { get; set; }
        public List<PedidoCompra> PedidoCompras { get; set; }
    }
}