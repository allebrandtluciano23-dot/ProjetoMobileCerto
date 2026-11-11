using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela de Perfil: nome, e-mail e foto do usuário logado
    public class PerfilViewModel : BaseViewModel
    {
        private readonly UsuarioDAO usuarioDAO = new UsuarioDAO();

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

        private string? sCaminhoFoto;
        public string? CaminhoFoto
        {
            get => sCaminhoFoto;
            set
            {
                if (SetProperty(ref sCaminhoFoto, value))
                {
                    OnPropertyChanged(nameof(TemFoto));
                    OnPropertyChanged(nameof(SemFoto));
                }
            }
        }

        public bool TemFoto => !string.IsNullOrEmpty(CaminhoFoto);
        public bool SemFoto => !TemFoto;

        /// <summary>Mostra os dados de quem está logado.</summary>
        public void Carregar()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            Nome = usuario?.Nome ?? "";
            Email = usuario?.Email ?? "";
            CaminhoFoto = usuario?.CaminhoFoto;
        }

        /// <summary>
        /// Abre a galeria e troca a foto do perfil.
        /// Retorna true se trocou (false se cancelou ou deu erro).
        /// </summary>
        public async Task<bool> AlterarFotoAsync()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            if (usuario == null || bProcessando)
                return false;

            MensagemErro = "";
            bProcessando = true;
            try
            {
                string? sNovaFoto = await FotoHelper.EscolherECopiarAsync("perfil");
                if (sNovaFoto == null)
                    return false; // cancelou

                string? sFotoAntiga = usuario.CaminhoFoto;

                await usuarioDAO.AlterarFotoAsync(usuario.IdUsuario, sNovaFoto);

                usuario.CaminhoFoto = sNovaFoto; // atualiza a sessão (tela inicial e menu usam)
                CaminhoFoto = sNovaFoto;

                FotoHelper.Apagar(sFotoAntiga);
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível alterar a foto.";
                return false;
            }
            finally
            {
                bProcessando = false;
            }
        }
    }
}
