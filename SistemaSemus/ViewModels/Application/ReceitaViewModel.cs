using System.ComponentModel.DataAnnotations;

namespace SistemaSemus.ViewModels.Application
{
    public class ReceitaViewModel
    {
        [Display(Name = "CRM"), StringLength(13)]
        public string MedicoID { get; set; }
        public string NomeMedico { get; set; }
        [Display(Name = "CPF")]
        public string PacienteID { get; set; }
        public string NomePaciente { get; set; }
        public string UserID { get; set; }
    }
}