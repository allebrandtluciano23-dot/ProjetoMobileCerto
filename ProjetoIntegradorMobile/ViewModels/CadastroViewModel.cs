using System.Text.RegularExpressions;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela de cadastro (CadastroPage)
    public class CadastroViewModel : BaseViewModel
    {
        private const int TAMANHO_MINIMO_SENHA = 6;

        private readonly UsuarioDAO usuarioDAO = new UsuarioDAO();

        // ---------- Campos da tela (ligados por Binding) ----------

        private string sNome = "";
        public string Nome
        {
            get => sNome;
            set => SetProperty(ref sNome, value);
        }

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

        private string sConfirmarSenha = "";
        public string ConfirmarSenha
        {
            get => sConfirmarSenha;
            set => SetProperty(ref sConfirmarSenha, value);
        }

        // ---------- Ações ----------

        /// <summary>
        /// Valida o formulário e cadastra o usuário.
        /// Retorna true se deu certo (o usuário já fica logado e a tela navega para a página inicial).
        /// </summary>
        public async Task<bool> CadastrarAsync()
        {
            if (bProcessando)
                return false;

            MensagemErro = "";

            string sNomeDigitado = Nome?.Trim() ?? "";
            string sEmailDigitado = Email?.Trim() ?? "";
            string sSenhaDigitada = Senha ?? "";
            string sConfirmacao = ConfirmarSenha ?? "";

            // ----- Validação do formulário -----
            if (string.IsNullOrWhiteSpace(sNomeDigitado) ||
                string.IsNullOrWhiteSpace(sEmailDigitado) ||
                string.IsNullOrWhiteSpace(sSenhaDigitada))
            {
                MensagemErro = "Preencha todos os campos.";
                return false;
            }

            if (!EmailValido(sEmailDigitado))
            {
                MensagemErro = "Digite um e-mail válido.";
                return false;
            }

            if (sSenhaDigitada.Length < TAMANHO_MINIMO_SENHA)
            {
                MensagemErro = $"A senha deve ter pelo menos {TAMANHO_MINIMO_SENHA} caracteres.";
                return false;
            }

            if (sSenhaDigitada != sConfirmacao)
            {
                MensagemErro = "As senhas não coincidem.";
                return false;
            }

            // ----- Gravação no banco -----
            bProcessando = true;
            try
            {
                Usuario? usuario = await usuarioDAO.CadastrarAsync(sNomeDigitado, sEmailDigitado, sSenhaDigitada);
                if (usuario == null)
                {
                    MensagemErro = "Já existe uma conta com esse e-mail.";
                    return false;
                }

                SessaoUsuario.UsuarioAtual = usuario;
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível salvar o cadastro. Tente novamente.";
                return false;
            }
            finally
            {
                bProcessando = false;
            }
        }

        // Formato simples: algo@algo.algo, sem espaços
        private static bool EmailValido(string sEmail)
        {
            return Regex.IsMatch(sEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
