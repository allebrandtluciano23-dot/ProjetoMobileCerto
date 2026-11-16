using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    // Ligação cópia-ingrediente (PK composta: alterar/excluir via SQL)
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
