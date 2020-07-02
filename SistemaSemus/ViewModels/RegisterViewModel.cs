using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaSemus.ViewModels
{
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        public string Nome { get; set; }
        [Required, StringLength(20), Display(Name = "Celular")]
        public string Fone { get; set; }

        [Required, Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Required, Display(Name = "Função")]
        public int Funcao { get; set; }

        [Required]
        public int Setor { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Nascimento { get; set; }

        [Required, StringLength(50), Display(Name = "Usuário")]
        public string UserName { get; set; }

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Senha")]
        [StringLength(100, ErrorMessage = "O/A {0} deve ter no mínimo {2} caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password), Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "A senha e a senha de confirmação não correspondem.")]
        public string ConfirmPassword { get; set; }
    }

}