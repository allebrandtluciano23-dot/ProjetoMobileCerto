using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class Ingrediente
    {
        [PrimaryKey, AutoIncrement]
        public int IdIngrediente { get; set; }

        [NotNull, Unique]
        public string Nome { get; set; } = string.Empty;

        public string UnidadePadrao { get; set; } = string.Empty;
    }
}
