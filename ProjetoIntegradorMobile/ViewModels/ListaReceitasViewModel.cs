using System.Collections.ObjectModel;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // Tipo de lista
    public enum TipoLista
    {
        Favoritas,
        CriadasPorMim
    }

    // ViewModel de Favoritos e Criadas por mim
    public class ListaReceitasViewModel : BaseViewModel
    {
        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();
        private readonly TipoLista tipo;

        public ListaReceitasViewModel(TipoLista tipo)
        {
            this.tipo = tipo;
        }

        public ObservableCollection<ReceitaResumo> Receitas { get; } = new ObservableCollection<ReceitaResumo>();

        // Texto da lista vazia
        private string sMensagemVazia = "";
        public string MensagemVazia
        {
            get => sMensagemVazia;
            set => SetProperty(ref sMensagemVazia, value);
        }

        // Carrega a lista do banco
        public async Task CarregarAsync()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            if (usuario == null)
            {
                Receitas.Clear();
                MensagemVazia = tipo == TipoLista.Favoritas
                    ? "Entre na sua conta para ver os seus favoritos."
                    : "Entre na sua conta para ver as suas receitas.";
                return;
            }

            MensagemVazia = tipo == TipoLista.Favoritas
                ? "Você ainda não favoritou nenhuma receita. Abra uma receita e toque em Favoritar."
                : "Você ainda não criou nenhuma receita. Toque em + Nova receita para começar.";

            try
            {
                List<ReceitaResumo> lReceitas = await receitaDAO.ListarResumoAsync(
                    usuario.IdUsuario,
                    bSoFavoritas: tipo == TipoLista.Favoritas,
                    bSoDoUsuario: tipo == TipoLista.CriadasPorMim);

                // Limpa só depois de ler o banco (evita lista duplicada)
                Receitas.Clear();
                foreach (ReceitaResumo receita in lReceitas)
                    Receitas.Add(receita);

                MensagemErro = "";
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar a lista.";
            }
        }
    }
}
