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
            if (usuario == null)
            {
                Adaptacoes.Clear();
                return;
            }

            try
            {
                List<AdaptacaoResumo> lAdaptacoes = await adaptacaoDAO.ListarResumoDoUsuarioAsync(usuario.IdUsuario);

                // Só limpa DEPOIS de ler do banco (evita lista duplicada quando duas recargas se cruzam)
                Adaptacoes.Clear();
                foreach (AdaptacaoResumo adaptacao in lAdaptacoes)
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
