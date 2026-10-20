namespace ProjetoIntegradorMobile.Models
{
    // Não é tabela: é o resultado da consulta que junta receita_ingrediente com ingrediente,
    // para mostrar na tela o nome do ingrediente junto com quantidade e unidade.
    public class IngredienteDaReceita
    {
        public int IdIngrediente { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Quantidade { get; set; }
        public string? Unidade { get; set; }

        public string Descricao => $"{Quantidade:0.##} {Unidade} {Nome}".Replace("  ", " ").Trim();
    }
}
