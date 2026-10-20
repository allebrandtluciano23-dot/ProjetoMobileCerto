using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("categoria")]
    public class Categoria
    {
        [PrimaryKey, AutoIncrement, Column("id_categoria")]
        public int IdCategoria { get; set; }

        [NotNull, Unique, Column("nome")]
        public string Nome { get; set; } = string.Empty;
    }
}
