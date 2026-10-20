using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    [Table("refeicao")]
    public class Refeicao
    {
        [PrimaryKey, AutoIncrement, Column("id_refeicao")]
        public int IdRefeicao { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; } // FK

        [Column("id_plano")]
        public int? IdPlano { get; set; } // FK

        // Preencha só UM destes dois (regra CHECK da tabela)
        [Column("id_receita")]
        public int? IdReceita { get; set; } // FK

        [Column("id_receita_personalizada")]
        public int? IdReceitaPersonalizada { get; set; } // FK

        // Valores aceitos: cafe_da_manha, almoco, jantar, lanche, ceia
        [NotNull, Column("tipo_refeicao")]
        public string TipoRefeicao { get; set; } = string.Empty;

        [NotNull, Column("data")]
        public string Data { get; set; } = string.Empty;

        [Column("horario")]
        public string? Horario { get; set; }
    }
}
