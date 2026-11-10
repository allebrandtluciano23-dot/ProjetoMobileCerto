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
        public bool Vegetariana { get; set; }

        // Favorita DO USUÁRIO LOGADO (vem da tabela favorito, não da receita)
        public bool Favorita { get; set; }
        public string? Dificuldade { get; set; }
        public string? CaminhoFoto { get; set; }

        public bool TemFoto => !string.IsNullOrEmpty(CaminhoFoto);

        // Nomes dos ingredientes separados por espaço (usado só pelo filtro de busca)
        public string? NomesIngredientes { get; set; }

        // Texto do card, ex.: "40 min · 2 porções · Fácil"
        public string Detalhes => MontarDetalhes(TempoPreparoMin, Porcoes, Dificuldade);

        // Também usado pela tela de detalhe da receita
        public static string MontarDetalhes(int? iTempoMin, int? iPorcoes, string? sDificuldade)
        {
            List<string> lPartes = new List<string>();

            if (iTempoMin > 0)
                lPartes.Add($"{iTempoMin} min");

            if (iPorcoes == 1)
                lPartes.Add("1 porção");
            else if (iPorcoes > 1)
                lPartes.Add($"{iPorcoes} porções");

            if (!string.IsNullOrEmpty(sDificuldade))
                lPartes.Add(sDificuldade);

            return string.Join(" · ", lPartes);
        }
    }
}
