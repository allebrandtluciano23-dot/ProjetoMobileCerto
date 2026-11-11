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
        // A primeira receita do resultado vai para o cartão grande (Destaque); as outras ficam aqui.
        public ObservableCollection<ReceitaResumo> Receitas { get; } = new ObservableCollection<ReceitaResumo>();

        // Receita mostrada no cartão grande do topo (null = nenhuma receita encontrada)
        private ReceitaResumo? receitaDestaque;
        public ReceitaResumo? Destaque
        {
            get => receitaDestaque;
            set
            {
                if (SetProperty(ref receitaDestaque, value))
                {
                    OnPropertyChanged(nameof(TemDestaque));
                    OnPropertyChanged(nameof(SemReceitas));
                }
            }
        }

        public bool TemDestaque => Destaque != null;

        // Mostra "Nenhuma receita encontrada" (banco vazio ou busca sem resultado)
        public bool SemReceitas => Destaque == null;

        // ---------- Campos da tela (ligados por Binding) ----------

        private string sSaudacao = "Olá, Chefe!";
        public string Saudacao
        {
            get => sSaudacao;
            set => SetProperty(ref sSaudacao, value);
        }

        // Foto do usuário logado no topo da tela (sem foto: mostra o desenho do chapéu de chef)
        private string? sFotoUsuario;
        public string? FotoUsuario
        {
            get => sFotoUsuario;
            set
            {
                if (SetProperty(ref sFotoUsuario, value))
                {
                    OnPropertyChanged(nameof(TemFotoUsuario));
                    OnPropertyChanged(nameof(SemFotoUsuario));
                }
            }
        }

        public bool TemFotoUsuario => !string.IsNullOrEmpty(FotoUsuario);
        public bool SemFotoUsuario => !TemFotoUsuario;

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
                // O coração dos itens mostra as favoritas de quem está logado (0 = ninguém logado)
                int iIdUsuario = SessaoUsuario.UsuarioAtual?.IdUsuario ?? 0;
                lReceitasTodas = await receitaDAO.ListarResumoAsync(iIdUsuario);
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
            ReceitaResumo? primeira = null;

            foreach (ReceitaResumo receita in lReceitasTodas)
            {
                bool bCategoriaOk = CategoriaSelecionada == "" || receita.NomeCategoria == CategoriaSelecionada;

                bool bTextoOk = sTermo == ""
                    || receita.Titulo.ToLowerInvariant().Contains(sTermo)
                    || (receita.NomesIngredientes ?? "").ToLowerInvariant().Contains(sTermo);

                if (!bCategoriaOk || !bTextoOk)
                    continue;

                // A primeira que combinar vai para o cartão grande; as demais, para a lista
                if (primeira == null)
                    primeira = receita;
                else
                    Receitas.Add(receita);
            }

            Destaque = primeira;
        }

        // "Olá, Luciano!" se houver alguém logado; senão "Olá, Chefe!"
        private void AtualizarSaudacao()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            FotoUsuario = usuario?.CaminhoFoto;

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
