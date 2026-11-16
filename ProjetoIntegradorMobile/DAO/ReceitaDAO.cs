using ProjetoIntegradorMobile.Data;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.DAO
{
    // Acesso às tabelas receita e receita_ingrediente
    public class ReceitaDAO
    {
        // Create

        // Salva a receita e os ingredientes numa transação; devolve o id
        public async Task<int> InserirAsync(
            Receita receita,
            IEnumerable<(string Nome, double Quantidade, string? Unidade)> ingredientes)
        {
            var db = await DatabaseConfig.Database.Conexao();
            if (string.IsNullOrEmpty(receita.DataCriacao))
                receita.DataCriacao = Database.Agora();

            await db.RunInTransactionAsync(conn =>
            {
                conn.Insert(receita);
                SalvarIngredientes(conn, receita.IdReceita, ingredientes);
            });

            return receita.IdReceita;
        }

        // Read

        public async Task<Receita?> BuscarPorIdAsync(int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.FindAsync<Receita>(iIdReceita);
        }

        // Lista para os cards (filtros: só favoritas / só do usuário)
        public async Task<List<ReceitaResumo>> ListarResumoAsync(int iIdUsuarioLogado, bool bSoFavoritas = false, bool bSoDoUsuario = false)
        {
            var db = await DatabaseConfig.Database.Conexao();

            // 1º "?" = id do usuário logado (favorita)
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

        // Quantas pessoas capturaram a receita
        public async Task<int> ContarCapturasAsync(int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM receita_personalizada WHERE id_receita_original = ?", iIdReceita);
        }

        // Update

        // Atualiza a receita e troca os ingredientes
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

        // Delete

        // Apaga a receita (ingredientes vão junto)
        public async Task<int> ExcluirAsync(int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.DeleteAsync<Receita>(iIdReceita);
        }

        // Auxiliar

        // Grava os ingredientes, criando os que não existem
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
