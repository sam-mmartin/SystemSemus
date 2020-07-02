namespace SistemaSemus.Models
{
    public class PedidoProduto
    {
        public int ID { get; set; }
        public int PedidoEstoqueID { get; set; }
        public int? ProdutoID { get; set; }
        public int? ProdutoSemEstoqueID { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }

        public virtual PedidoEstoque PedidoEstoque { get; set; }
    }
}