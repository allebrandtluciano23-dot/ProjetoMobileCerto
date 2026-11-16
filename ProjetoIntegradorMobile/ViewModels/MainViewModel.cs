using System.Collections.ObjectModel;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela inicial
    public class MainViewModel : BaseViewModel
    {
        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();

        // Todas as receitas do banco (base do filtro)
        private List<ReceitaResumo> lReceitasTodas = new List<ReceitaResumo>();

        // Receitas da lista (sem a do destaque)
        public ObservableCollection<ReceitaResumo> Receitas { get; } = new ObservableCollection<ReceitaResumo>();

        // Receita do cartão grande
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

        // Mostra "Nenhuma receita encontrada"
        public bool SemReceitas => Destaque == null;

        // Campos da tela

        private string sSaudacao = "Olá, Chefe!";
        public string Saudacao
        {
            get => sSaudacao;
            set => SetProperty(ref sSaudacao, value);
        }

        // Foto do usuário logado
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

        // Texto da busca: filtra a cada letra
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

        // Categoria escolhida ("" = todas)
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

        // Ações

        // Carrega as receitas do banco
        public async Task CarregarAsync()
        {
            AtualizarSaudacao();

            try
            {
                // 0 = ninguém logado
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

        // Filtra por texto (título e ingredientes) e categoria
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

                // 1ª vai para o destaque, o resto para a lista
                if (primeira == null)
                    primeira = receita;
                else
                    Receitas.Add(receita);
            }

            Destaque = primeira;
        }

        // "Olá, <nome>!" ou "Olá, Chefe!"
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
