namespace ProjetoIntegradorMobile.Models
{
    // Não é tabela: resultado da consulta que junta a adaptação (cópia capturada)
    // com a receita original e o autor dela. Usado na lista "Minhas adaptações".
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

        // Ex.: Baseada em "Sopa de legumes" · por CheefBook
        public string Origem => string.IsNullOrEmpty(NomeAutorOriginal)
            ? $"Baseada em \"{TituloOriginal}\""
            : $"Baseada em \"{TituloOriginal}\" · por {NomeAutorOriginal}";

        public string Detalhes => ReceitaResumo.MontarDetalhes(TempoPreparoMin, Porcoes, Dificuldade);
    }
}
