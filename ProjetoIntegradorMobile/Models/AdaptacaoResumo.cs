namespace ProjetoIntegradorMobile.Models
{
    // Resumo da cópia capturada para a lista (não é tabela)
    public class AdaptacaoResumo
    {
        public int IdReceitaPersonalizada { get; set; }
        public int IdReceitaOriginal { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string TituloOriginal { get; set; } = string.Empty;
        public string? NomeAutorOriginal { get; set; }
        public string? CaminhoFoto { get; set; }
        public int? TempoPreparoMin { get; set; }
        public int? Porcoes { get; set; }
        public string? Dificuldade { get; set; }

        public bool TemFoto => !string.IsNullOrEmpty(CaminhoFoto);

        // Ex.: Baseada em "Sopa" · por Ana
        public string Origem => string.IsNullOrEmpty(NomeAutorOriginal)
            ? $"Baseada em \"{TituloOriginal}\""
            : $"Baseada em \"{TituloOriginal}\" · por {NomeAutorOriginal}";

        public string Detalhes => ReceitaResumo.MontarDetalhes(TempoPreparoMin, Porcoes, Dificuldade);
    }
}
