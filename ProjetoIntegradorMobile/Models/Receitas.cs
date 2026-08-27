using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class Receita
    {
        [PrimaryKey, AutoIncrement]
        public int IdReceita { get; set; }

        public int IdUsuario { get; set; } // FK

        public int? IdCategoria { get; set; } // FK

        [NotNull]
        public string Titulo { get; set; } = string.Empty;

        [NotNull]
        public string ModoPreparo { get; set; } = string.Empty;

        public int? TempoPreparoMin { get; set; }

        public int? Porcoes { get; set; }

        [NotNull]
        public string DataCriacao { get; set; } = string.Empty;
    }
}