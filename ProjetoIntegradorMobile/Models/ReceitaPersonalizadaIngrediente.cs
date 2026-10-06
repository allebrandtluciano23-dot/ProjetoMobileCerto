using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class ReceitaPersonalizadaIngrediente
    {
        public int IdReceitaPersonalizada { get; set; } // FK

        public int IdIngrediente { get; set; } // FK

        [NotNull]
        public double Quantidade { get; set; }

        public string Unidade { get; set; } = string.Empty;
    }
}