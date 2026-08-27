using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class Refeicao
    {
        [PrimaryKey, AutoIncrement]
        public int IdRefeicao { get; set; }

        public int IdUsuario { get; set; } // FK

        public int? IdPlano { get; set; } // FK

        public int? IdReceita { get; set; } // FK

        public int? IdReceitaPersonalizada { get; set; } // FK

        [NotNull]
        public string TipoRefeicao { get; set; } = string.Empty;

        [NotNull]
        public string Data { get; set; } = string.Empty;

        public string Horario { get; set; } = string.Empty;
    }
}