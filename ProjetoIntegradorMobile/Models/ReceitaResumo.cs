namespace ProjetoIntegradorMobile.Models
{
    // Resumo da receita para os cards (não é tabela)
    public class ReceitaResumo
    {
        public int IdReceita { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? NomeCategoria { get; set; }
        public int? TempoPreparoMin { get; set; }
        public int? Porcoes { get; set; }
        public bool Vegetariana { get; set; }

        // Favorita do usuário logado
        public bool Favorita { get; set; }
        public string? Dificuldade { get; set; }
        public string? CaminhoFoto { get; set; }

        public bool TemFoto => !string.IsNullOrEmpty(CaminhoFoto);
        public bool SemFoto => !TemFoto;

        // Nomes dos ingredientes (para a busca)
        public string? NomesIngredientes { get; set; }

        // Ex.: "40 min · 2 porções · Fácil"
        public string Detalhes => MontarDetalhes(TempoPreparoMin, Porcoes, Dificuldade);

        // Ex.: "Almoço · 40 min · 2 porções · Fácil"
        public string CategoriaEDetalhes
        {
            get
            {
                if (string.IsNullOrEmpty(NomeCategoria))
                    return Detalhes;

                if (string.IsNullOrEmpty(Detalhes))
                    return NomeCategoria;

                return $"{NomeCategoria} · {Detalhes}";
            }
        }

        // Também usado no detalhe
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
