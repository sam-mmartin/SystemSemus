using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class EstoqueSemus
    {
        public int ID { get; set; }
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataAtualizacao { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        // Navegação
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<ProdutoEstoqueSemus> ProdutoEstoqueSemus { get; set; }
        public virtual ICollection<PedidoEstoque> PedidoEstoques { get; set; }
        public virtual ICollection<PedidoCompra> PedidoCompras { get; set; }
    }
}