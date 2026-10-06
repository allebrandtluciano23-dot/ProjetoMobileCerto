using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class ReceitaPersonalizada
    {
        [PrimaryKey, AutoIncrement]
        public int IdReceitaPersonalizada { get; set; }

        public int IdReceitaOriginal { get; set; } // FK

        public int IdUsuario { get; set; } // FK

        [NotNull]
        public string Titulo { get; set; } = string.Empty;

        [NotNull]
        public string ModoPreparo { get; set; } = string.Empty;

        public string Observacoes { get; set; } = string.Empty;

        [NotNull]
        public string DataCaptura { get; set; } = string.Empty;
    }
}
