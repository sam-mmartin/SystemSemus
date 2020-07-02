using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class Medico
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "CRM"), StringLength(13)]
        public string ID { get; set; }
        [StringLength(50)]
        public string Nome { get; set; }
        [StringLength(100)]
        [Display(Name = "Áreas de Atuação/Especialidades")]
        public string Area_Atuacao { get; set; }
        [Display(Name = "Data do Cadastro")]
        public DateTime DataCadastro { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Receita> Receitas { get; set; }
    }
}