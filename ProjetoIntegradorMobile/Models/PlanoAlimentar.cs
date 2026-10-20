using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("plano_alimentar")]
    public class PlanoAlimentar
    {
        [PrimaryKey, AutoIncrement, Column("id_plano")]
        public int IdPlano { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; } // FK

        [NotNull, Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("data_inicio")]
        public string? DataInicio { get; set; }

        [Column("data_fim")]
        public string? DataFim { get; set; }
    }
}
