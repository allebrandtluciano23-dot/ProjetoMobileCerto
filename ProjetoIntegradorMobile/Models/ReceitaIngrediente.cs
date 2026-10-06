using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class ReceitaIngrediente
    {
        public int IdReceita { get; set; } // FK

        public int IdIngrediente { get; set; } // FK

        [NotNull]
        public double Quantidade { get; set; }

        public string Unidade { get; set; } = string.Empty;
    }
}