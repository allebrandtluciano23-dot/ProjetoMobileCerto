using SQLite;

namespace ProjetoIntegradorMobile.Models
{
    // Tabela de ligação com chave primária composta (id_receita, id_ingrediente).
    // O sqlite-net não suporta PK composta: dá para usar InsertAsync e QueryAsync,
    // mas UpdateAsync/DeleteAsync(objeto) não funcionam. Para alterar/excluir use SQL, ex.:
    // ExecuteAsync("DELETE FROM receita_ingrediente WHERE id_receita = ? AND id_ingrediente = ?", ...)
    [Table("receita_ingrediente")]
    public class ReceitaIngrediente
    {
        [Column("id_receita")]
        public int IdReceita { get; set; } // FK

        [Column("id_ingrediente")]
        public int IdIngrediente { get; set; } // FK

        [NotNull, Column("quantidade")]
        public double Quantidade { get; set; }

        [Column("unidade")]
        public string? Unidade { get; set; }
    }
}
