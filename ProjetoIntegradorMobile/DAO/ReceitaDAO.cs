using ProjetoIntegradorMobile.Data;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.DAO
{
    // Acesso aos dados das tabelas "receita" e "receita_ingrediente"
    public class ReceitaDAO
    {
        // ---------- CREATE ----------

        /// <summary>
        /// Salva a receita e os ingredientes de uma vez (se algo falhar, nada é salvo).
        /// Ingredientes que ainda não existem são criados automaticamente.
        /// Retorna o id da receita criada.
        /// </summary>
        public async Task<int> InserirAsync(
            Receita receita,
            IEnumerable<(string Nome, double Quantidade, string? Unidade)> ingredientes)
        {
            var db = await DatabaseConfig.Database.Conexao();
            if (string.IsNullOrEmpty(receita.DataCriacao))
                receita.DataCriacao = Database.Agora();

            await db.RunInTransactionAsync(conn =>
            {
                conn.Insert(receita); // preenche receita.IdReceita
                SalvarIngredientes(conn, receita.IdReceita, ingredientes);
            });

            return receita.IdReceita;
        }

        // ---------- READ ----------

        public async Task<Receita?> BuscarPorIdAsync(int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.FindAsync<Receita>(iIdReceita);
        }

        public async Task<List<Receita>> ListarAsync()
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.QueryAsync<Receita>(
                "SELECT * FROM receita ORDER BY data_criacao DESC");
        }

        public async Task<List<Receita>> ListarPorUsuarioAsync(int iIdUsuario)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.QueryAsync<Receita>(
                "SELECT * FROM receita WHERE id_usuario = ? ORDER BY data_criacao DESC", iIdUsuario);
        }

        public async Task<List<Receita>> ListarPorCategoriaAsync(int iIdCategoria)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.QueryAsync<Receita>(
                "SELECT * FROM receita WHERE id_categoria = ? ORDER BY titulo", iIdCategoria);
        }

        public async Task<List<Receita>> ListarFavoritasAsync(int iIdUsuario)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.QueryAsync<Receita>(
                "SELECT * FROM receita WHERE id_usuario = ? AND favorita = 1 ORDER BY titulo", iIdUsuario);
        }

        /// <summary>Filtro da busca: receitas cujo título ou algum ingrediente contém o texto.</summary>
        public async Task<List<Receita>> BuscarAsync(string sTexto)
        {
            var db = await DatabaseConfig.Database.Conexao();
            string sTermo = "%" + sTexto.Trim() + "%";
            return await db.QueryAsync<Receita>(@"
                SELECT DISTINCT r.* FROM receita r
                LEFT JOIN receita_ingrediente ri ON ri.id_receita = r.id_receita
                LEFT JOIN ingrediente i ON i.id_ingrediente = ri.id_ingrediente
                WHERE r.titulo LIKE ? OR i.nome LIKE ?
                ORDER BY r.titulo", sTermo, sTermo);
        }

        public async Task<List<IngredienteDaReceita>> ListarIngredientesAsync(int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.QueryAsync<IngredienteDaReceita>(@"
                SELECT i.id_ingrediente AS IdIngrediente, i.nome AS Nome,
                       ri.quantidade AS Quantidade, ri.unidade AS Unidade
                FROM receita_ingrediente ri
                JOIN ingrediente i ON i.id_ingrediente = ri.id_ingrediente
                WHERE ri.id_receita = ?
                ORDER BY i.nome", iIdReceita);
        }

        // ---------- UPDATE ----------

        /// <summary>
        /// Atualiza a receita e troca a lista de ingredientes pela nova (tudo de uma vez).
        /// </summary>
        public async Task AtualizarAsync(
            Receita receita,
            IEnumerable<(string Nome, double Quantidade, string? Unidade)> ingredientes)
        {
            var db = await DatabaseConfig.Database.Conexao();

            await db.RunInTransactionAsync(conn =>
            {
                conn.Update(receita);
                conn.Execute("DELETE FROM receita_ingrediente WHERE id_receita = ?", receita.IdReceita);
                SalvarIngredientes(conn, receita.IdReceita, ingredientes);
            });
        }

        // Usado pelo CheckBox "Favorita" (liga/desliga sem mexer no resto da receita)
        public async Task AlterarFavoritaAsync(int iIdReceita, bool bFavorita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            await db.ExecuteAsync(
                "UPDATE receita SET favorita = ? WHERE id_receita = ?", bFavorita ? 1 : 0, iIdReceita);
        }

        // ---------- DELETE ----------

        // Apaga a receita e, em cascata, os ingredientes ligados a ela
        public async Task<int> ExcluirAsync(int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.DeleteAsync<Receita>(iIdReceita);
        }

        // ---------- AUXILIAR ----------

        // Grava os ingredientes da receita, criando na tabela "ingrediente" os que ainda não existem.
        // Roda dentro da transação de InserirAsync/AtualizarAsync.
        private static void SalvarIngredientes(
            SQLite.SQLiteConnection conn,
            int iIdReceita,
            IEnumerable<(string Nome, double Quantidade, string? Unidade)> ingredientes)
        {
            foreach (var item in ingredientes)
            {
                string sNome = item.Nome.Trim();
                Ingrediente? ingrediente = conn.Table<Ingrediente>().FirstOrDefault(i => i.Nome == sNome);
                if (ingrediente == null)
                {
                    ingrediente = new Ingrediente { Nome = sNome, UnidadePadrao = item.Unidade };
                    conn.Insert(ingrediente);
                }

                conn.Execute(
                    "INSERT OR REPLACE INTO receita_ingrediente (id_receita, id_ingrediente, quantidade, unidade) VALUES (?, ?, ?, ?)",
                    iIdReceita, ingrediente.IdIngrediente, item.Quantidade, item.Unidade);
            }
        }
    }
}
