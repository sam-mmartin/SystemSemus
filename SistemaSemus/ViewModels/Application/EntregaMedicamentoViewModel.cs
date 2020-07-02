using System.Collections.Generic;

namespace SistemaSemus.ViewModels.Application
{
    public class EntregaViewModel
    {
        public int ReceitaMedicaID { get; set; }
        public EntregaMedicamentoViewModel EntregaMedicamento { get; set; }
        public ICollection<DisponibilidadeProdutoViewModel> DisponibilidadeProdutos { get; set; }
    }

    public class EntregaMedicamentoViewModel
    {
        private ICollection<MedicamentoViewModel> medicamentoViewModels = new List<MedicamentoViewModel>();

        public int EstoqueID { get; set; }
        public string MedicoID { get; set; }
        public string PacienteID { get; set; }
        public string NomeMedico { get; set; }
        public string NomePaciente { get; set; }

        public virtual ICollection<MedicamentoViewModel> MedicamentoViewModels { get => medicamentoViewModels; set => medicamentoViewModels = value; }
    }

    public class MedicamentoViewModel
    {
        public int? ProdutoID { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
    }

    public class DisponibilidadeProdutoViewModel
    {
        private ICollection<StockProductQuantidadeViewModel> stockProductQuantidades = new List<StockProductQuantidadeViewModel>();

        public int ID { get; set; }
        public string Descricao { get; set; }
        public virtual ICollection<StockProductQuantidadeViewModel> StockProductQuantidades { get => stockProductQuantidades; set => stockProductQuantidades = value; }
    }

    public class StockProductQuantidadeViewModel
    {
        public string Stock { get; set; }
        public int Quantidade { get; set; }
    }
}