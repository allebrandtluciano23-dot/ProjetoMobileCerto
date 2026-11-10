using System.Collections.ObjectModel;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da lista "Minhas adaptações" (receitas que o usuário capturou)
    public class CapturasViewModel : BaseViewModel
    {
        private readonly AdaptacaoDAO adaptacaoDAO = new AdaptacaoDAO();

        public ObservableCollection<AdaptacaoResumo> Adaptacoes { get; } = new ObservableCollection<AdaptacaoResumo>();

        /// <summary>Lê as cópias do usuário logado. Chamado toda vez que a tela aparece.</summary>
        public async Task CarregarAsync()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            Adaptacoes.Clear();
            if (usuario == null)
                return;

            try
            {
                foreach (AdaptacaoResumo adaptacao in await adaptacaoDAO.ListarResumoDoUsuarioAsync(usuario.IdUsuario))
                    Adaptacoes.Add(adaptacao);
                MensagemErro = "";
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar as suas adaptações.";
            }
        }
    }
}
