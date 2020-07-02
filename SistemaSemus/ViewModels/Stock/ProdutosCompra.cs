using SistemaSemus.Models;
using System.Collections.Generic;

namespace SistemaSemus.ViewModels
{
    public class ProdutosCompra
    {
        public List<PedidoProduto> PedidoProdutos { get; set; }
        public List<ProdutoNaoCadastrado> ProdutoNaoCadastrados { get; set; }
    }
}