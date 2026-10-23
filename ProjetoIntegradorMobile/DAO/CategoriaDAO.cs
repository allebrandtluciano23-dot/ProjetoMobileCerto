using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.DAO
{
    // Acesso aos dados da tabela "categoria"
    public class CategoriaDAO
    {
        public async Task<List<Categoria>> ListarAsync()
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.Table<Categoria>().OrderBy(c => c.Nome).ToListAsync();
        }

        public async Task<Categoria?> BuscarPorIdAsync(int iIdCategoria)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.FindAsync<Categoria>(iIdCategoria);
        }

        public async Task<Categoria?> BuscarPorNomeAsync(string sNome)
        {
            var db = await DatabaseConfig.Database.Conexao();
            sNome = sNome.Trim();
            return await db.Table<Categoria>().FirstOrDefaultAsync(c => c.Nome == sNome);
        }
    }
}
