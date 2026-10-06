using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class PlanoAlimentar
    {
        [PrimaryKey, AutoIncrement]
        public int IdPlano { get; set; }

        public int IdUsuario { get; set; } // FK

        [NotNull]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string DataInicio { get; set; } = string.Empty;

        public string DataFim { get; set; } = string.Empty;
    }
}