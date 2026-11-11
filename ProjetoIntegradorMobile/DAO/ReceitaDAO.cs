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

        /// <summary>
        /// Lista de receitas para os cards: dados da receita + nome da categoria
        /// + nomes dos ingredientes (para o filtro) + se é favorita DO USUÁRIO LOGADO.
        /// iIdUsuarioLogado = 0 quando ninguém está logado (aí nenhuma aparece como favorita).
        /// bSoFavoritas = true  -> só as favoritas desse usuário (tela Favoritos).
        /// bSoDoUsuario = true  -> só as receitas criadas por esse usuário (tela Minhas receitas).
        /// Mais recentes primeiro.
        /// </summary>
        public async Task<List<ReceitaResumo>> ListarResumoAsync(int iIdUsuarioLogado, bool bSoFavoritas = false, bool bSoDoUsuario = false)
        {
            var db = await DatabaseConfig.Database.Conexao();

            // O primeiro "?" é o do EXISTS (favorita) e sempre recebe o id do usuário logado
            List<object> lParametros = new List<object> { iIdUsuarioLogado };
            List<string> lFiltros = new List<string>();

            if (bSoFavoritas)
            {
                lFiltros.Add("EXISTS (SELECT 1 FROM favorito f WHERE f.id_receita = r.id_receita AND f.id_usuario = ?)");
                lParametros.Add(iIdUsuarioLogado);
            }

            if (bSoDoUsuario)
            {
                lFiltros.Add("r.id_usuario = ?");
                lParametros.Add(iIdUsuarioLogado);
            }

            string sWhere = lFiltros.Count > 0 ? "WHERE " + string.Join(" AND ", lFiltros) : "";

            string sSql = @"
                SELECT r.id_receita        AS IdReceita,
                       r.titulo            AS Titulo,
                       c.nome              AS NomeCategoria,
                       r.tempo_preparo_min AS TempoPreparoMin,
                       r.porcoes           AS Porcoes,
                       r.vegetariana       AS Vegetariana,
                       r.dificuldade       AS Dificuldade,
                       r.caminho_foto      AS CaminhoFoto,
                       EXISTS (SELECT 1 FROM favorito f
                                WHERE f.id_receita = r.id_receita AND f.id_usuario = ?) AS Favorita,
                       (SELECT GROUP_CONCAT(i.nome, ' ')
                          FROM receita_ingrediente ri
                          JOIN ingrediente i ON i.id_ingrediente = ri.id_ingrediente
                         WHERE ri.id_receita = r.id_receita) AS NomesIngredientes
                FROM receita r
                LEFT JOIN categoria c ON c.id_categoria = r.id_categoria
                " + sWhere + @"
                ORDER BY r.data_criacao DESC, r.id_receita DESC";

            return await db.QueryAsync<ReceitaResumo>(sSql, lParametros.ToArray());
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

        /// <summary>Quantas pessoas capturaram (copiaram) esta receita. Usado no aviso antes de excluir.</summary>
        public async Task<int> ContarCapturasAsync(int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM receita_personalizada WHERE id_receita_original = ?", iIdReceita);
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
