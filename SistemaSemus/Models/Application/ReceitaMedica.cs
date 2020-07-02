using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SistemaSemus.Models.Application
{
    public class ReceitaMedica
    {
        public int ID { get; set; }

        [Display(Name = "Data de Entrada"), DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "CRM"), StringLength(13)]
        public string MedicoID { get; set; }

        [Display(Name = "CPF")]
        public string PacienteID { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }

        // Navegação
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Medico Medico { get; set; }
        public virtual Paciente Paciente { get; set; }

        public virtual ICollection<Prescricao> Prescricaos { get; set; }
    }
}