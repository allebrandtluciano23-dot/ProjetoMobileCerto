using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [PrimaryKey, AutoIncrement, Column("id_usuario")]
        public int IdUsuario { get; set; }

        [NotNull, Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [NotNull, Unique, Column("email")]
        public string Email { get; set; } = string.Empty;

        [NotNull, Column("senha_hash")]
        public string SenhaHash { get; set; } = string.Empty;

        // Foto do perfil copiada para a pasta do app (null = sem foto, mostra o 👨‍🍳)
        [Column("caminho_foto")]
        public string? CaminhoFoto { get; set; }

        // Mesmo formato do datetime('now') do SQLite (UTC)
        [NotNull, Column("data_cadastro")]
        public string DataCadastro { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
