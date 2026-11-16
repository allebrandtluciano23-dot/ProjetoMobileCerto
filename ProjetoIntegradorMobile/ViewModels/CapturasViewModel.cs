using System.Collections.ObjectModel;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da lista de cópias capturadas
    public class CapturasViewModel : BaseViewModel
    {
        private readonly AdaptacaoDAO adaptacaoDAO = new AdaptacaoDAO();

        public ObservableCollection<AdaptacaoResumo> Adaptacoes { get; } = new ObservableCollection<AdaptacaoResumo>();

        // Carrega as cópias do usuário logado
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

                // Limpa só depois de ler o banco (evita lista duplicada)
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
