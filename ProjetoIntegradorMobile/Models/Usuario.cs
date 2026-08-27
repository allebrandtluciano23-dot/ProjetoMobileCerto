using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int IdUsuario { get; set; }

        [NotNull]
        public string Nome { get; set; } = string.Empty;

        [NotNull, Unique]
        public string Email { get; set; } = string.Empty;

        [NotNull]
        public string SenhaHash { get; set; } = string.Empty;

        [NotNull]
        public string DataCadastro { get; set; } = string.Empty;
    }
}
