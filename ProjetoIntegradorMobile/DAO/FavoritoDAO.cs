namespace ProjetoIntegradorMobile.DAO
{
    // Acesso aos dados da tabela "favorito" (quais receitas cada usuário favoritou)
    public class FavoritoDAO
    {
        public async Task<bool> EhFavoritaAsync(int iIdUsuario, int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            int iTotal = await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM favorito WHERE id_usuario = ? AND id_receita = ?", iIdUsuario, iIdReceita);
            return iTotal > 0;
        }

        public async Task AdicionarAsync(int iIdUsuario, int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            await db.ExecuteAsync(
                "INSERT OR IGNORE INTO favorito (id_usuario, id_receita) VALUES (?, ?)", iIdUsuario, iIdReceita);
        }

        public async Task RemoverAsync(int iIdUsuario, int iIdReceita)
        {
            var db = await DatabaseConfig.Database.Conexao();
            await db.ExecuteAsync(
                "DELETE FROM favorito WHERE id_usuario = ? AND id_receita = ?", iIdUsuario, iIdReceita);
        }

        /// <summary>Liga/desliga o favorito. Retorna como ficou (true = agora é favorita).</summary>
        public async Task<bool> AlternarAsync(int iIdUsuario, int iIdReceita)
        {
            if (await EhFavoritaAsync(iIdUsuario, iIdReceita))
            {
                await RemoverAsync(iIdUsuario, iIdReceita);
                return false;
            }

            await AdicionarAsync(iIdUsuario, iIdReceita);
            return true;
        }
    }
}
