using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    // Tabela de ligação com chave primária composta (id_receita_personalizada, id_ingrediente).
    // Mesma observação de ReceitaIngrediente: para alterar/excluir, use SQL direto.
    [Table("receita_personalizada_ingrediente")]
    public class ReceitaPersonalizadaIngrediente
    {
        [Column("id_receita_personalizada")]
        public int IdReceitaPersonalizada { get; set; } // FK

        [Column("id_ingrediente")]
        public int IdIngrediente { get; set; } // FK

        [NotNull, Column("quantidade")]
        public double Quantidade { get; set; }

        [Column("unidade")]
        public string? Unidade { get; set; }
    }
}
