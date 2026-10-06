using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class Sugestao
    {
        [PrimaryKey, AutoIncrement]
        public int IdSugestao { get; set; }

        public int IdReceita { get; set; } // FK

        public int IdUsuario { get; set; } // FK

        public int? IdIngredienteReferenciado { get; set; } // FK

        public string EtapaReferenciada { get; set; } = string.Empty;

        [NotNull]
        public string Texto { get; set; } = string.Empty;

        [NotNull]
        public string DataEnvio { get; set; } = string.Empty;

        [NotNull]
        public string Status { get; set; } = "pendente";
    }
}