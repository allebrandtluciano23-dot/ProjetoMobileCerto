using System.Collections.ObjectModel;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // Qual lista a tela mostra
    public enum TipoLista
    {
        Favoritas,      // FavoritosPage: receitas que o usuário favoritou
        CriadasPorMim   // ReceitasCriadasPage: receitas que o usuário criou
    }

    // ViewModel das telas Favoritos e Minhas receitas (mesma lista, filtro diferente)
    public class ListaReceitasViewModel : BaseViewModel
    {
        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();
        private readonly TipoLista tipo;

        public ListaReceitasViewModel(TipoLista tipo)
        {
            this.tipo = tipo;
        }

        public ObservableCollection<ReceitaResumo> Receitas { get; } = new ObservableCollection<ReceitaResumo>();

        // Texto que aparece quando a lista está vazia (muda se não houver login)
        private string sMensagemVazia = "";
        public string MensagemVazia
        {
            get => sMensagemVazia;
            set => SetProperty(ref sMensagemVazia, value);
        }

        /// <summary>Lê a lista do banco. Chamado toda vez que a tela aparece.</summary>
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

                // Só limpa DEPOIS de ler do banco: se duas recargas acontecerem ao mesmo tempo
                // (ex.: ao fechar o detalhe da receita), cada uma troca a lista inteira de uma vez,
                // em vez de as duas adicionarem os mesmos itens (lista duplicada).
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
