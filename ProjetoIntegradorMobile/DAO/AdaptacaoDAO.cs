using ProjetoIntegradorMobile.Data;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.DAO
{
    /// <summary>
    /// Acesso aos dados das tabelas "receita_personalizada" e "receita_personalizada_ingrediente".
    /// Uma adaptação é a CÓPIA de uma receita capturada por outro usuário:
    /// quem capturou pode editar a cópia à vontade, e a receita original do autor não muda.
    /// </summary>
    public class AdaptacaoDAO
    {
        // ---------- CREATE (capturar) ----------

        /// <summary>
        /// Captura a receita: copia título, modo de preparo e ingredientes para uma adaptação do usuário.
        /// Se o usuário já tinha capturado essa receita, devolve a adaptação que já existe.
        /// Retorna o id da adaptação, ou 0 se a receita não existe.
        /// </summary>
        public async Task<int> CapturarAsync(int iIdReceitaOriginal, int iIdUsuario)
        {
            var db = await DatabaseConfig.Database.Conexao();

            int iIdExistente = await BuscarIdDoUsuarioAsync(iIdReceitaOriginal, iIdUsuario);
            if (iIdExistente > 0)
                return iIdExistente; // cada usuário captura cada receita uma vez só (UNIQUE no banco)

            Receita? original = await db.FindAsync<Receita>(iIdReceitaOriginal);
            if (original == null)
                return 0;

            ReceitaPersonalizada adaptacao = new ReceitaPersonalizada
            {
                IdReceitaOriginal = iIdReceitaOriginal,
                IdUsuario = iIdUsuario,
                Titulo = original.Titulo,
                ModoPreparo = original.ModoPreparo,
                DataCaptura = Database.Agora()
            };

            await db.RunInTransactionAsync(conn =>
            {
                conn.Insert(adaptacao); // preenche adaptacao.IdReceitaPersonalizada

                // Copia os ingredientes da original para a adaptação
                conn.Execute(@"
                    INSERT INTO receita_personalizada_ingrediente
                        (id_receita_personalizada, id_ingrediente, quantidade, unidade)
                    SELECT ?, id_ingrediente, quantidade, unidade
                    FROM receita_ingrediente WHERE id_receita = ?",
                    adaptacao.IdReceitaPersonalizada, iIdReceitaOriginal);
            });

            return adaptacao.IdReceitaPersonalizada;
        }

        // ---------- READ ----------

        /// <summary>Id da adaptação que o usuário fez dessa receita (0 se ainda não capturou).</summary>
        public async Task<int> BuscarIdDoUsuarioAsync(int iIdReceitaOriginal, int iIdUsuario)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.ExecuteScalarAsync<int>(
                "SELECT id_receita_personalizada FROM receita_personalizada WHERE id_receita_original = ? AND id_usuario = ?",
                iIdReceitaOriginal, iIdUsuario);
        }

        public async Task<ReceitaPersonalizada?> BuscarPorIdAsync(int iIdAdaptacao)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.FindAsync<ReceitaPersonalizada>(iIdAdaptacao);
        }

        /// <summary>Lista "Minhas adaptações": cópias do usuário + dados da receita original. Mais recentes primeiro.</summary>
        public async Task<List<AdaptacaoResumo>> ListarResumoDoUsuarioAsync(int iIdUsuario)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.QueryAsync<AdaptacaoResumo>(@"
                SELECT rp.id_receita_personalizada AS IdReceitaPersonalizada,
                       rp.id_receita_original      AS IdReceitaOriginal,
                       rp.titulo                   AS Titulo,
                       r.titulo                    AS TituloOriginal,
                       u.nome                      AS NomeAutorOriginal,
                       r.caminho_foto              AS CaminhoFoto,
                       r.tempo_preparo_min         AS TempoPreparoMin,
                       r.porcoes                   AS Porcoes,
                       r.dificuldade               AS Dificuldade
                FROM receita_personalizada rp
                JOIN receita r ON r.id_receita = rp.id_receita_original
                LEFT JOIN usuario u ON u.id_usuario = r.id_usuario
                WHERE rp.id_usuario = ?
                ORDER BY rp.data_captura DESC, rp.id_receita_personalizada DESC", iIdUsuario);
        }

        public async Task<List<IngredienteDaReceita>> ListarIngredientesAsync(int iIdAdaptacao)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.QueryAsync<IngredienteDaReceita>(@"
                SELECT i.id_ingrediente AS IdIngrediente, i.nome AS Nome,
                       rpi.quantidade AS Quantidade, rpi.unidade AS Unidade
                FROM receita_personalizada_ingrediente rpi
                JOIN ingrediente i ON i.id_ingrediente = rpi.id_ingrediente
                WHERE rpi.id_receita_personalizada = ?
                ORDER BY i.nome", iIdAdaptacao);
        }

        // ---------- UPDATE ----------

        /// <summary>
        /// Salva a adaptação e troca a lista de ingredientes pela nova (tudo de uma vez).
        /// Só mexe na cópia do usuário: a receita original não é alterada.
        /// </summary>
        public async Task AtualizarAsync(
            ReceitaPersonalizada adaptacao,
            IEnumerable<(string Nome, double Quantidade, string? Unidade)> ingredientes)
        {
            var db = await DatabaseConfig.Database.Conexao();

            await db.RunInTransactionAsync(conn =>
            {
                conn.Update(adaptacao);
                conn.Execute("DELETE FROM receita_personalizada_ingrediente WHERE id_receita_personalizada = ?",
                    adaptacao.IdReceitaPersonalizada);

                foreach (var item in ingredientes)
                {
                    // Ingrediente novo (que ainda não existe no banco) é criado
                    string sNome = item.Nome.Trim();
                    Ingrediente? ingrediente = conn.Table<Ingrediente>().FirstOrDefault(i => i.Nome == sNome);
                    if (ingrediente == null)
                    {
                        ingrediente = new Ingrediente { Nome = sNome, UnidadePadrao = item.Unidade };
                        conn.Insert(ingrediente);
                    }

                    conn.Execute(
                        "INSERT OR REPLACE INTO receita_personalizada_ingrediente (id_receita_personalizada, id_ingrediente, quantidade, unidade) VALUES (?, ?, ?, ?)",
                        adaptacao.IdReceitaPersonalizada, ingrediente.IdIngrediente, item.Quantidade, item.Unidade);
                }
            });
        }

        // ---------- DELETE ----------

        // Apaga só a cópia do usuário (e, em cascata, os ingredientes dela). A original não muda.
        public async Task<int> ExcluirAsync(int iIdAdaptacao)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.DeleteAsync<ReceitaPersonalizada>(iIdAdaptacao);
        }
    }
}
