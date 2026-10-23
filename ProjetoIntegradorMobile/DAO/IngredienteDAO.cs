using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.DAO
{
    // Acesso aos dados da tabela "ingrediente"
    public class IngredienteDAO
    {
        // ---------- CREATE ----------

        /// <summary>
        /// Insere um ingrediente novo.
        /// Retorna false se já existir um ingrediente com esse nome.
        /// </summary>
        public async Task<bool> InserirAsync(Ingrediente ingrediente)
        {
            var db = await DatabaseConfig.Database.Conexao();
            ingrediente.Nome = ingrediente.Nome.Trim();

            if (await NomeExisteAsync(ingrediente.Nome, 0))
                return false;

            await db.InsertAsync(ingrediente); // preenche ingrediente.IdIngrediente
            return true;
        }

        /// <summary>Devolve o ingrediente com esse nome, criando se ainda não existir.</summary>
        public async Task<Ingrediente> ObterOuCriarAsync(string sNome, string? sUnidadePadrao = null)
        {
            Ingrediente? existente = await BuscarPorNomeAsync(sNome);
            if (existente != null)
                return existente;

            var db = await DatabaseConfig.Database.Conexao();
            Ingrediente ingrediente = new Ingrediente { Nome = sNome.Trim(), UnidadePadrao = sUnidadePadrao };
            await db.InsertAsync(ingrediente);
            return ingrediente;
        }

        // ---------- READ ----------

        public async Task<List<Ingrediente>> ListarAsync()
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.Table<Ingrediente>().OrderBy(i => i.Nome).ToListAsync();
        }

        public async Task<Ingrediente?> BuscarPorIdAsync(int iIdIngrediente)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.FindAsync<Ingrediente>(iIdIngrediente);
        }

        public async Task<Ingrediente?> BuscarPorNomeAsync(string sNome)
        {
            var db = await DatabaseConfig.Database.Conexao();
            sNome = sNome.Trim();
            return await db.Table<Ingrediente>().FirstOrDefaultAsync(i => i.Nome == sNome);
        }

        /// <summary>Filtro da tela de ingredientes: nomes que contêm o texto digitado.</summary>
        public async Task<List<Ingrediente>> BuscarAsync(string sTexto)
        {
            var db = await DatabaseConfig.Database.Conexao();
            string sTermo = "%" + sTexto.Trim() + "%";
            return await db.QueryAsync<Ingrediente>(
                "SELECT * FROM ingrediente WHERE nome LIKE ? ORDER BY nome", sTermo);
        }

        // ---------- UPDATE ----------

        /// <summary>
        /// Atualiza o ingrediente.
        /// Retorna false se o novo nome já estiver sendo usado por outro ingrediente.
        /// </summary>
        public async Task<bool> AtualizarAsync(Ingrediente ingrediente)
        {
            var db = await DatabaseConfig.Database.Conexao();
            ingrediente.Nome = ingrediente.Nome.Trim();

            if (await NomeExisteAsync(ingrediente.Nome, ingrediente.IdIngrediente))
                return false;

            await db.UpdateAsync(ingrediente);
            return true;
        }

        // ---------- DELETE ----------

        /// <summary>
        /// Exclui o ingrediente.
        /// Retorna false se ele estiver sendo usado em alguma receita
        /// (o banco não deixa excluir: ON DELETE RESTRICT).
        /// </summary>
        public async Task<bool> ExcluirAsync(int iIdIngrediente)
        {
            var db = await DatabaseConfig.Database.Conexao();

            int iUsos = await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM receita_ingrediente WHERE id_ingrediente = ?", iIdIngrediente);
            if (iUsos > 0)
                return false;

            await db.DeleteAsync<Ingrediente>(iIdIngrediente);
            return true;
        }

        // ---------- AUXILIAR ----------

        // Confere se já existe outro ingrediente (id diferente) com o mesmo nome
        private async Task<bool> NomeExisteAsync(string sNome, int iIdIgnorar)
        {
            var db = await DatabaseConfig.Database.Conexao();
            int iTotal = await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM ingrediente WHERE nome = ? AND id_ingrediente <> ?", sNome, iIdIgnorar);
            return iTotal > 0;
        }
    }
}
