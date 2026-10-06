using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class Categoria
    {
        [PrimaryKey, AutoIncrement]
        public int IdCategoria { get; set; }

        [NotNull, Unique]
        public string Nome { get; set; } = string.Empty;
    }
}