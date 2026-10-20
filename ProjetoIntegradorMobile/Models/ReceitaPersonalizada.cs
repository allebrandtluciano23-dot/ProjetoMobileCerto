using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("receita_personalizada")]
    public class ReceitaPersonalizada
    {
        [PrimaryKey, AutoIncrement, Column("id_receita_personalizada")]
        public int IdReceitaPersonalizada { get; set; }

        [Column("id_receita_original")]
        public int IdReceitaOriginal { get; set; } // FK

        [Column("id_usuario")]
        public int IdUsuario { get; set; } // FK

        [NotNull, Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [NotNull, Column("modo_preparo")]
        public string ModoPreparo { get; set; } = string.Empty;

        [Column("observacoes")]
        public string? Observacoes { get; set; }

        [NotNull, Column("data_captura")]
        public string DataCaptura { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
