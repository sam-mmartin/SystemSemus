using SistemaSemus.Models.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSemus.Models
{
    public class Paciente
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "CPF")]
        public string ID { get; set; }
        [StringLength(50)]
        public string Nome { get; set; }
        [StringLength(20)]
        public string RG { get; set; }
        [Display(Name = "Orgão Emissor"), StringLength(50)]
        public string Orgao_Emissor { get; set; }

        [Display(Name = "Data de Nascimento"), DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        public virtual ICollection<Receita> Receitas { get; set; }
        public virtual ICollection<ReceitaMedica> ReceitaMedicas { get; set; }
    }
}