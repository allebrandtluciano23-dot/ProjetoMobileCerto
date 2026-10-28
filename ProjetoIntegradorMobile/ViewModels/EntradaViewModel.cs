using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela de login (EntradaPage)
    public class EntradaViewModel : BaseViewModel
    {
        private readonly UsuarioDAO usuarioDAO = new UsuarioDAO();

        // ---------- Campos da tela (ligados por Binding) ----------

        private string sEmail = "";
        public string Email
        {
            get => sEmail;
            set => SetProperty(ref sEmail, value);
        }

        private string sSenha = "";
        public string Senha
        {
            get => sSenha;
            set => SetProperty(ref sSenha, value);
        }

        // ---------- Ações ----------

        /// <summary>
        /// Valida os campos e confere e-mail e senha no banco.
        /// Retorna true se o login deu certo (a tela então navega para a página inicial).
        /// </summary>
        public async Task<bool> EntrarAsync()
        {
            if (bProcessando)
                return false;

            MensagemErro = "";

            string sEmailDigitado = Email?.Trim() ?? "";
            string sSenhaDigitada = Senha ?? "";

            if (string.IsNullOrWhiteSpace(sEmailDigitado) || string.IsNullOrWhiteSpace(sSenhaDigitada))
            {
                MensagemErro = "Preencha e-mail e senha.";
                return false;
            }

            bProcessando = true;
            try
            {
                Usuario? usuario = await usuarioDAO.AutenticarAsync(sEmailDigitado, sSenhaDigitada);
                if (usuario == null)
                {
                    MensagemErro = "E-mail ou senha inválidos.";
                    return false;
                }

                SessaoUsuario.UsuarioAtual = usuario;
                Senha = ""; // não deixa a senha na tela depois de entrar
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível acessar o banco de dados. Tente novamente.";
                return false;
            }
            finally
            {
                bProcessando = false;
            }
        }
    }
}
