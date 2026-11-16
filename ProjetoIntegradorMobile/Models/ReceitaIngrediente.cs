using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    // Ligação receita-ingrediente (PK composta: alterar/excluir via SQL)
    [Table("receita_ingrediente")]
    public class ReceitaIngrediente
    {
        [Column("id_receita")]
        public int IdReceita { get; set; } // FK

        [Column("id_ingrediente")]
        public int IdIngrediente { get; set; } // FK

        [NotNull, Column("quantidade")]
        public double Quantidade { get; set; }

        [Column("unidade")]
        public string? Unidade { get; set; }
    }
}
