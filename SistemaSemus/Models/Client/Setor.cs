using System.ComponentModel.DataAnnotations;

namespace SistemaSemus.Models.Client
{
    public class Setor
    {
        public int ID { get; set; }
        [Required, StringLength(50)]
        public string Descricao { get; set; }
    }
}