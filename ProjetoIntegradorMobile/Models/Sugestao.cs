using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("sugestao")]
    public class Sugestao
    {
        [PrimaryKey, AutoIncrement, Column("id_sugestao")]
        public int IdSugestao { get; set; }

        [Column("id_receita")]
        public int IdReceita { get; set; } // FK

        [Column("id_usuario")]
        public int IdUsuario { get; set; } // FK

        [Column("id_ingrediente_referenciado")]
        public int? IdIngredienteReferenciado { get; set; } // FK

        [Column("etapa_referenciada")]
        public string? EtapaReferenciada { get; set; }

        [NotNull, Column("texto")]
        public string Texto { get; set; } = string.Empty;

        [NotNull, Column("data_envio")]
        public string DataEnvio { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        // Valores aceitos: pendente, aceita, rejeitada
        [NotNull, Column("status")]
        public string Status { get; set; } = "pendente";
    }
}
