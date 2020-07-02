using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SistemaSemus.Models.Client;

namespace SistemaSemus.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(50)]
        public string Nome { get; set; }
        [Required, Display(Name = "Endereço")]
        public string Endereco { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime Nascimento { get; set; }
        [Required, ForeignKey("Funcao")]
        public int FuncaoID { get; set; }
        [Required, ForeignKey("Setor")]
        public int SetorID { get; set; }

        public virtual Funcao Funcao { get; set; }
        public virtual Setor Setor { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Observe que o authenticationType deve corresponder àquele definido em CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim("Nome", Nome));
            userIdentity.AddClaim(new Claim("Funcao", Funcao.Descricao));
            userIdentity.AddClaim(new Claim("Setor", Setor.Descricao));
            return userIdentity;
        }
    }
}