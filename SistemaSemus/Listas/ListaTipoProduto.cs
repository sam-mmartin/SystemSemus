using System.Collections.Generic;

namespace SistemaSemus.Listas
{
    public class TipoProduto
    {
        public int ID { get; set; }
        public string Descricao { get; set; }

        public List<TipoProduto> ListaTipoProduto()
        {
            return new List<TipoProduto>
            {
                new TipoProduto{ID=1, Descricao="Medicamento"},
                new TipoProduto{ID=2, Descricao="Suprimento"}
            };
        }
    }

    public class TipoBuscaReceita
    {
        public int ID { get; set; }
        public string Descricao { get; set; }

        public List<TipoBuscaReceita> ListaTipoBuscaReceita()
        {
            return new List<TipoBuscaReceita>
            {
                new TipoBuscaReceita{ID=1, Descricao="CPF do Paciente"},
                new TipoBuscaReceita{ID=2, Descricao="Nº da Receita Médica"}
            };
        }
    }
}