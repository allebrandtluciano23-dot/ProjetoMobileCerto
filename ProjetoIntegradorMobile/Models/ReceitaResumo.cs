namespace ProjetoIntegradorMobile.Models
{
    // Não é tabela: é o resultado da consulta que junta receita + categoria + ingredientes,
    // com só o que o card da lista de receitas precisa mostrar.
    public class ReceitaResumo
    {
        public int IdReceita { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? NomeCategoria { get; set; }
        public int? TempoPreparoMin { get; set; }
        public int? Porcoes { get; set; }
        public bool Favorita { get; set; }
        public string? Dificuldade { get; set; }
        public string? CaminhoFoto { get; set; }

        public bool TemFoto => !string.IsNullOrEmpty(CaminhoFoto);

        // Nomes dos ingredientes separados por espaço (usado só pelo filtro de busca)
        public string? NomesIngredientes { get; set; }

        // Texto do card, ex.: "40 min · 2 porções · Fácil"
        public string Detalhes
        {
            get
            {
                List<string> lPartes = new List<string>();

                if (TempoPreparoMin > 0)
                    lPartes.Add($"{TempoPreparoMin} min");

                if (Porcoes == 1)
                    lPartes.Add("1 porção");
                else if (Porcoes > 1)
                    lPartes.Add($"{Porcoes} porções");

                if (!string.IsNullOrEmpty(Dificuldade))
                    lPartes.Add(Dificuldade);

                return string.Join(" · ", lPartes);
            }
        }
    }
}
