using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("ingrediente")]
    public class Ingrediente
    {
        [PrimaryKey, AutoIncrement, Column("id_ingrediente")]
        public int IdIngrediente { get; set; }

        [NotNull, Unique, Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("unidade_padrao")]
        public string? UnidadePadrao { get; set; }
    }
}
