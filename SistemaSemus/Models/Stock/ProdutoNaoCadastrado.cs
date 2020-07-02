using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaSemus.Models
{
    public class ProdutoNaoCadastrado
    {
        public int ID { get; set; }
        public byte TipoProduto { get; set; }
        public int Quantidade { get; set; }
        public string Descricao { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataEntrada { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DataPedido { get; set; }
    }
}