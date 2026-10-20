using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("receita")]
    public class Receita
    {
        [PrimaryKey, AutoIncrement, Column("id_receita")]
        public int IdReceita { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; } // FK

        [Column("id_categoria")]
        public int? IdCategoria { get; set; } // FK

        [NotNull, Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [NotNull, Column("modo_preparo")]
        public string ModoPreparo { get; set; } = string.Empty;

        [Column("tempo_preparo_min")]
        public int? TempoPreparoMin { get; set; }

        [Column("porcoes")]
        public int? Porcoes { get; set; }

        [NotNull, Column("data_criacao")]
        public string DataCriacao { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
