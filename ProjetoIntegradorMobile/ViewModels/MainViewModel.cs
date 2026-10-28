using System.Collections.ObjectModel;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela inicial (MainPage): lista de receitas com filtro em tempo real
    public class MainViewModel : BaseViewModel
    {
        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();

        // Todas as receitas lidas do banco (o filtro trabalha em cima desta lista)
        private List<ReceitaResumo> lReceitasTodas = new List<ReceitaResumo>();

        // O que aparece na tela (CollectionView). ObservableCollection avisa a tela quando muda.
        public ObservableCollection<ReceitaResumo> Receitas { get; } = new ObservableCollection<ReceitaResumo>();

        // ---------- Campos da tela (ligados por Binding) ----------

        private string sSaudacao = "Olá, Chefe!";
        public string Saudacao
        {
            get => sSaudacao;
            set => SetProperty(ref sSaudacao, value);
        }

        // Texto da barra de pesquisa: a cada letra digitada, a lista é filtrada
        private string sFiltro = "";
        public string Filtro
        {
            get => sFiltro;
            set
            {
                if (SetProperty(ref sFiltro, value))
                    AplicarFiltro();
            }
        }

        // Categoria escolhida nos botões (Todos, Rápidos, Almoço...). "" = todas
        private string sCategoriaSelecionada = "";
        public string CategoriaSelecionada
        {
            get => sCategoriaSelecionada;
            set
            {
                if (SetProperty(ref sCategoriaSelecionada, value))
                    AplicarFiltro();
            }
        }

        // ---------- Ações ----------

        /// <summary>Lê as receitas do banco. Chamado toda vez que a tela aparece.</summary>
        public async Task CarregarAsync()
        {
            AtualizarSaudacao();

            try
            {
                lReceitasTodas = await receitaDAO.ListarResumoAsync();
                MensagemErro = "";
            }
            catch (Exception)
            {
                lReceitasTodas = new List<ReceitaResumo>();
                MensagemErro = "Não foi possível carregar as receitas.";
            }

            AplicarFiltro();
        }

        // Mostra na tela só as receitas que combinam com o texto digitado e a categoria escolhida.
        // O texto é procurado no título e nos ingredientes (ex.: "frango" acha receitas com frango).
        private void AplicarFiltro()
        {
            string sTermo = (Filtro ?? "").Trim().ToLowerInvariant();

            Receitas.Clear();

            foreach (ReceitaResumo receita in lReceitasTodas)
            {
                bool bCategoriaOk = CategoriaSelecionada == "" || receita.NomeCategoria == CategoriaSelecionada;

                bool bTextoOk = sTermo == ""
                    || receita.Titulo.ToLowerInvariant().Contains(sTermo)
                    || (receita.NomesIngredientes ?? "").ToLowerInvariant().Contains(sTermo);

                if (bCategoriaOk && bTextoOk)
                    Receitas.Add(receita);
            }
        }

        // "Olá, Luciano!" se houver alguém logado; senão "Olá, Chefe!"
        private void AtualizarSaudacao()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Nome))
            {
                Saudacao = "Olá, Chefe!";
                return;
            }

            string sPrimeiroNome = usuario.Nome.Trim().Split(' ')[0];
            Saudacao = $"Olá, {sPrimeiroNome}!";
        }
    }
}
