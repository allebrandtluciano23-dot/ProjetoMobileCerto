namespace ProjetoIntegradorMobile.Models
{
    // Ingrediente com quantidade e unidade (não é tabela)
    public class IngredienteDaReceita
    {
        public int IdIngrediente { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Quantidade { get; set; }
        public string? Unidade { get; set; }

        public string Descricao => $"{Quantidade:0.##} {Unidade} {Nome}".Replace("  ", " ").Trim();

        // Ex.: "400 g"
        public string QuantidadeTexto => $"{Quantidade:0.##} {Unidade}".Trim();
    }
}
