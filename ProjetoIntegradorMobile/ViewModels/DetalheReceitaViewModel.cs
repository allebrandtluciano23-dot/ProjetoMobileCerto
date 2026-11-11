using System.Collections.ObjectModel;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela de detalhe da receita (DetalheReceitaPage, com abas)
    public class DetalheReceitaViewModel : BaseViewModel
    {
        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();
        private readonly CategoriaDAO categoriaDAO = new CategoriaDAO();
        private readonly UsuarioDAO usuarioDAO = new UsuarioDAO();
        private readonly FavoritoDAO favoritoDAO = new FavoritoDAO();
        private readonly AdaptacaoDAO adaptacaoDAO = new AdaptacaoDAO();

        public int IdReceita { get; }

        public DetalheReceitaViewModel(int iIdReceita)
        {
            IdReceita = iIdReceita;
        }

        // ---------- Dados mostrados nas abas (ligados por Binding) ----------

        private string sTitulo = "";
        public string Titulo
        {
            get => sTitulo;
            set => SetProperty(ref sTitulo, value);
        }

        private string sNomeCategoria = "";
        public string NomeCategoria
        {
            get => sNomeCategoria;
            set => SetProperty(ref sNomeCategoria, value);
        }

        // Ex.: "40 min · 2 porções · Fácil"
        private string sDetalhes = "";
        public string Detalhes
        {
            get => sDetalhes;
            set => SetProperty(ref sDetalhes, value);
        }

        // Os três cartõezinhos do Resumo: tempo, porções e dificuldade ("—" quando não informado)
        private string sTextoTempo = "—";
        public string TextoTempo
        {
            get => sTextoTempo;
            set => SetProperty(ref sTextoTempo, value);
        }

        private string sTextoPorcoes = "—";
        public string TextoPorcoes
        {
            get => sTextoPorcoes;
            set => SetProperty(ref sTextoPorcoes, value);
        }

        private string sTextoDificuldade = "—";
        public string TextoDificuldade
        {
            get => sTextoDificuldade;
            set => SetProperty(ref sTextoDificuldade, value);
        }

        // Linha abaixo do título na aba Ingredientes, ex.: "6 ingredientes · 2 porções"
        private string sResumoIngredientes = "";
        public string ResumoIngredientes
        {
            get => sResumoIngredientes;
            set => SetProperty(ref sResumoIngredientes, value);
        }

        // Ex.: "Por Luciano"
        private string sAutor = "";
        public string Autor
        {
            get => sAutor;
            set => SetProperty(ref sAutor, value);
        }

        private string sModoPreparo = "";
        public string ModoPreparo
        {
            get => sModoPreparo;
            set => SetProperty(ref sModoPreparo, value);
        }

        private bool bVegetariana = false;
        public bool Vegetariana
        {
            get => bVegetariana;
            set => SetProperty(ref bVegetariana, value);
        }

        // Favorita DO USUÁRIO LOGADO (tabela favorito)
        private bool bFavorita = false;
        public bool Favorita
        {
            get => bFavorita;
            set
            {
                if (SetProperty(ref bFavorita, value))
                    OnPropertyChanged(nameof(TextoFavorito));
            }
        }

        public string TextoFavorito => Favorita ? "Favorita" : "Favoritar";

        // Qualquer usuário logado pode favoritar (inclusive receitas de outros usuários)
        private bool bEstaLogado = false;
        public bool EstaLogado
        {
            get => bEstaLogado;
            set => SetProperty(ref bEstaLogado, value);
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

        // Só quem criou a receita vê os botões Editar e Excluir
        private bool bEhAutor = false;
        public bool EhAutor
        {
            get => bEhAutor;
            set => SetProperty(ref bEhAutor, value);
        }

        // ---------- Captura ----------

        // Quem NÃO é o autor (e está logado) pode capturar a receita
        private bool bPodeCapturar = false;
        public bool PodeCapturar
        {
            get => bPodeCapturar;
            set => SetProperty(ref bPodeCapturar, value);
        }

        // Id da cópia que o usuário logado já capturou (0 = ainda não capturou)
        private int iIdAdaptacao = 0;
        public int IdAdaptacao
        {
            get => iIdAdaptacao;
            set
            {
                if (SetProperty(ref iIdAdaptacao, value))
                    OnPropertyChanged(nameof(TextoCaptura));
            }
        }

        public string TextoCaptura => IdAdaptacao > 0 ? "Minha versão" : "Capturar";

        public ObservableCollection<IngredienteDaReceita> Ingredientes { get; } = new ObservableCollection<IngredienteDaReceita>();

        // ---------- Ações ----------

        /// <summary>
        /// Lê a receita, a categoria, o autor e os ingredientes do banco.
        /// Retorna false se a receita não existe mais (ex.: foi excluída).
        /// </summary>
        /// <summary>
        /// Botão de favoritar: liga/desliga o favorito do usuário logado.
        /// Retorna a mensagem para o Toast (ou "" se não deu).
        /// </summary>
        public async Task<string> AlternarFavoritaAsync()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            if (usuario == null || bProcessando)
                return "";

            bProcessando = true;
            try
            {
                Favorita = await favoritoDAO.AlternarAsync(usuario.IdUsuario, IdReceita);
                return Favorita ? "Adicionada aos favoritos" : "Removida dos favoritos";
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível atualizar os favoritos.";
                return "";
            }
            finally
            {
                bProcessando = false;
            }
        }

        /// <summary>
        /// Captura a receita (cria a cópia do usuário logado) ou, se já capturou, devolve a cópia existente.
        /// Retorna o id da cópia, ou 0 se não deu.
        /// </summary>
        public async Task<int> CapturarAsync()
        {
            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            if (usuario == null || !PodeCapturar || bProcessando)
                return 0;

            if (IdAdaptacao > 0)
                return IdAdaptacao;

            bProcessando = true;
            try
            {
                IdAdaptacao = await adaptacaoDAO.CapturarAsync(IdReceita, usuario.IdUsuario);
                if (IdAdaptacao == 0)
                    MensagemErro = "Esta receita não existe mais.";
                return IdAdaptacao;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível capturar a receita.";
                return 0;
            }
            finally
            {
                bProcessando = false;
            }
        }

        public async Task<bool> CarregarAsync()
        {
            try
            {
                Receita? receita = await receitaDAO.BuscarPorIdAsync(IdReceita);
                if (receita == null)
                    return false;

                Categoria? categoria = receita.IdCategoria.HasValue
                    ? await categoriaDAO.BuscarPorIdAsync(receita.IdCategoria.Value)
                    : null;
                Usuario? autor = await usuarioDAO.BuscarPorIdAsync(receita.IdUsuario);

                Titulo = receita.Titulo;
                NomeCategoria = categoria?.Nome ?? "";
                Detalhes = ReceitaResumo.MontarDetalhes(receita.TempoPreparoMin, receita.Porcoes, receita.Dificuldade);
                TextoTempo = receita.TempoPreparoMin > 0 ? $"{receita.TempoPreparoMin} min" : "—";
                TextoPorcoes = receita.Porcoes > 0 ? receita.Porcoes.ToString()! : "—";
                TextoDificuldade = string.IsNullOrEmpty(receita.Dificuldade) ? "—" : receita.Dificuldade;
                Autor = autor != null ? $"por {autor.Nome}" : "";
                ModoPreparo = receita.ModoPreparo;
                Vegetariana = receita.Vegetariana;
                CaminhoFoto = receita.CaminhoFoto;

                Usuario? usuarioLogado = SessaoUsuario.UsuarioAtual;
                EstaLogado = usuarioLogado != null;
                EhAutor = usuarioLogado?.IdUsuario == receita.IdUsuario;
                Favorita = usuarioLogado != null
                    && await favoritoDAO.EhFavoritaAsync(usuarioLogado.IdUsuario, IdReceita);

                PodeCapturar = usuarioLogado != null && !EhAutor;
                IdAdaptacao = PodeCapturar
                    ? await adaptacaoDAO.BuscarIdDoUsuarioAsync(IdReceita, usuarioLogado!.IdUsuario)
                    : 0;

                List<IngredienteDaReceita> lIngredientes = await receitaDAO.ListarIngredientesAsync(IdReceita);
                Ingredientes.Clear();
                foreach (IngredienteDaReceita ingrediente in lIngredientes)
                    Ingredientes.Add(ingrediente);

                string sQtdIngredientes = lIngredientes.Count == 1 ? "1 ingrediente" : $"{lIngredientes.Count} ingredientes";
                ResumoIngredientes = TextoPorcoes == "—" ? sQtdIngredientes : $"{sQtdIngredientes} · {TextoPorcoes} porções";

                MensagemErro = "";
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar a receita.";
                return true; // a tela continua aberta mostrando o erro
            }
        }
    }
}
