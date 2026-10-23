using ProjetoIntegradorMobile.Data;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.DAO
{
    // Acesso aos dados da tabela "usuario"
    public class UsuarioDAO
    {
        /// <summary>
        /// Cadastra um usuário guardando a senha com hash.
        /// Retorna null se o e-mail já estiver cadastrado.
        /// </summary>
        public async Task<Usuario?> CadastrarAsync(string sNome, string sEmail, string sSenha)
        {
            var db = await DatabaseConfig.Database.Conexao();
            sEmail = sEmail.Trim().ToLowerInvariant();

            if (await EmailExisteAsync(sEmail))
                return null;

            Usuario usuario = new Usuario
            {
                Nome = sNome.Trim(),
                Email = sEmail,
                SenhaHash = SenhaHelper.GerarHash(sSenha),
                DataCadastro = Database.Agora()
            };

            await db.InsertAsync(usuario); // preenche usuario.IdUsuario
            return usuario;
        }

        /// <summary>
        /// Confere e-mail e senha. Retorna o usuário se estiverem corretos, senão null.
        /// </summary>
        public async Task<Usuario?> AutenticarAsync(string sEmail, string sSenha)
        {
            Usuario? usuario = await BuscarPorEmailAsync(sEmail);
            if (usuario == null || !SenhaHelper.Verificar(sSenha, usuario.SenhaHash))
                return null;
            return usuario;
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string sEmail)
        {
            var db = await DatabaseConfig.Database.Conexao();
            sEmail = sEmail.Trim().ToLowerInvariant();
            return await db.Table<Usuario>().FirstOrDefaultAsync(u => u.Email == sEmail);
        }

        public async Task<Usuario?> BuscarPorIdAsync(int iIdUsuario)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.FindAsync<Usuario>(iIdUsuario);
        }

        public async Task<bool> EmailExisteAsync(string sEmail)
        {
            var db = await DatabaseConfig.Database.Conexao();
            sEmail = sEmail.Trim().ToLowerInvariant();
            return await db.Table<Usuario>().CountAsync(u => u.Email == sEmail) > 0;
        }

        public async Task<int> AtualizarAsync(Usuario usuario)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.UpdateAsync(usuario);
        }

        public async Task<bool> AlterarSenhaAsync(int iIdUsuario, string sSenhaAtual, string sNovaSenha)
        {
            var db = await DatabaseConfig.Database.Conexao();
            Usuario? usuario = await db.FindAsync<Usuario>(iIdUsuario);
            if (usuario == null || !SenhaHelper.Verificar(sSenhaAtual, usuario.SenhaHash))
                return false;

            usuario.SenhaHash = SenhaHelper.GerarHash(sNovaSenha);
            await db.UpdateAsync(usuario);
            return true;
        }

        // Apaga o usuário e, em cascata, tudo o que é dele (receitas etc.)
        public async Task<int> ExcluirAsync(int iIdUsuario)
        {
            var db = await DatabaseConfig.Database.Conexao();
            return await db.DeleteAsync<Usuario>(iIdUsuario);
        }
    }
}
