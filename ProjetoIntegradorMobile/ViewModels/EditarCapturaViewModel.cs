using System.Collections.ObjectModel;
using System.Globalization;
using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela "Minha versão" (EditarCapturaPage): edição da cópia capturada.
    // Tudo o que é salvo aqui fica só na cópia de quem capturou; a receita original não muda.
    public class EditarCapturaViewModel : BaseViewModel
    {
        private const int TAMANHO_MINIMO_TITULO = 3;
        private const int TAMANHO_MAXIMO_TITULO = 80;

        private readonly AdaptacaoDAO adaptacaoDAO = new AdaptacaoDAO();
        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();
        private readonly UsuarioDAO usuarioDAO = new UsuarioDAO();

        private readonly int iIdAdaptacao;
        private bool bCarregou = false;

        public EditarCapturaViewModel(int iIdAdaptacao)
        {
            this.iIdAdaptacao = iIdAdaptacao;
        }

        // ---------- Dados da receita original (só para mostrar) ----------

        private string sOrigem = "";
        public string Origem
        {
            get => sOrigem;
            set => SetProperty(ref sOrigem, value);
        }

        private string sDetalhes = "";
        public string Detalhes
        {
            get => sDetalhes;
            set => SetProperty(ref sDetalhes, value);
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

        // ---------- Campos editáveis da cópia (ligados por Binding) ----------

        private string sTitulo = "";
        public string Titulo
        {
            get => sTitulo;
            set => SetProperty(ref sTitulo, value);
        }

        private string sModoPreparo = "";
        public string ModoPreparo
        {
            get => sModoPreparo;
            set => SetProperty(ref sModoPreparo, value);
        }

        // Ex.: "Troquei o açúcar por mel"
        private string sObservacoes = "";
        public string Observacoes
        {
            get => sObservacoes;
            set => SetProperty(ref sObservacoes, value);
        }

        // ---------- Ingredientes (mesmo funcionamento da Nova receita) ----------

        public ObservableCollection<IngredienteDaReceita> Ingredientes { get; } = new ObservableCollection<IngredienteDaReceita>();

        public List<string> Unidades { get; } = new List<string>
        {
            "g", "kg", "ml", "l", "un", "colher (sopa)", "colher (chá)", "xícara", "pitada"
        };

        private string sNovoIngredienteNome = "";
        public string NovoIngredienteNome
        {
            get => sNovoIngredienteNome;
            set => SetProperty(ref sNovoIngredienteNome, value);
        }

        private string sNovoIngredienteQuantidade = "";
        public string NovoIngredienteQuantidade
        {
            get => sNovoIngredienteQuantidade;
            set => SetProperty(ref sNovoIngredienteQuantidade, value);
        }

        private string? sNovoIngredienteUnidade;
        public string? NovoIngredienteUnidade
        {
            get => sNovoIngredienteUnidade;
            set => SetProperty(ref sNovoIngredienteUnidade, value);
        }

        private string sErroIngrediente = "";
        public string ErroIngrediente
        {
            get => sErroIngrediente;
            set
            {
                if (SetProperty(ref sErroIngrediente, value))
                    OnPropertyChanged(nameof(TemErroIngrediente));
            }
        }

        public bool TemErroIngrediente => !string.IsNullOrEmpty(ErroIngrediente);

        // ---------- Ações ----------

        /// <summary>
        /// Lê a cópia, os ingredientes dela e os dados da original (só na primeira vez).
        /// Retorna false se a cópia não existe mais ou não é do usuário logado.
        /// </summary>
        public async Task<bool> CarregarAsync()
        {
            if (bCarregou)
                return true;

            try
            {
                ReceitaPersonalizada? adaptacao = await adaptacaoDAO.BuscarPorIdAsync(iIdAdaptacao);
                if (adaptacao == null || adaptacao.IdUsuario != SessaoUsuario.UsuarioAtual?.IdUsuario)
                    return false;

                Titulo = adaptacao.Titulo;
                ModoPreparo = adaptacao.ModoPreparo;
                Observacoes = adaptacao.Observacoes ?? "";

                Ingredientes.Clear();
                foreach (IngredienteDaReceita ingrediente in await adaptacaoDAO.ListarIngredientesAsync(iIdAdaptacao))
                    Ingredientes.Add(ingrediente);

                // Dados da original: tempo, porções, dificuldade e foto vêm dela
                Receita? original = await receitaDAO.BuscarPorIdAsync(adaptacao.IdReceitaOriginal);
                if (original != null)
                {
                    Usuario? autor = await usuarioDAO.BuscarPorIdAsync(original.IdUsuario);
                    Origem = autor != null
                        ? $"Baseada em \"{original.Titulo}\" · por {autor.Nome}"
                        : $"Baseada em \"{original.Titulo}\"";
                    Detalhes = ReceitaResumo.MontarDetalhes(original.TempoPreparoMin, original.Porcoes, original.Dificuldade);
                    CaminhoFoto = original.CaminhoFoto;
                }

                bCarregou = true;
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar a sua versão.";
                return true;
            }
        }

        /// <summary>Valida os campos de ingrediente e adiciona na lista.</summary>
        public void AdicionarIngrediente()
        {
            ErroIngrediente = "";

            string sNome = NovoIngredienteNome?.Trim() ?? "";
            string sQuantidade = (NovoIngredienteQuantidade ?? "").Trim().Replace(',', '.');

            if (sNome == "")
            {
                ErroIngrediente = "Digite o nome do ingrediente.";
                return;
            }

            if (!double.TryParse(sQuantidade, NumberStyles.Float, CultureInfo.InvariantCulture, out double dQuantidade)
                || dQuantidade <= 0)
            {
                ErroIngrediente = "Digite uma quantidade válida (ex.: 200 ou 1,5).";
                return;
            }

            if (string.IsNullOrEmpty(NovoIngredienteUnidade))
            {
                ErroIngrediente = "Escolha a unidade.";
                return;
            }

            if (Ingredientes.Any(i => string.Equals(i.Nome, sNome, StringComparison.OrdinalIgnoreCase)))
            {
                ErroIngrediente = "Esse ingrediente já está na lista.";
                return;
            }

            Ingredientes.Add(new IngredienteDaReceita
            {
                Nome = sNome,
                Quantidade = dQuantidade,
                Unidade = NovoIngredienteUnidade
            });

            NovoIngredienteNome = "";
            NovoIngredienteQuantidade = "";
            NovoIngredienteUnidade = null;
        }

        public void RemoverIngrediente(IngredienteDaReceita ingrediente)
        {
            Ingredientes.Remove(ingrediente);
        }

        /// <summary>Valida e salva a cópia. Retorna true se salvou.</summary>
        public async Task<bool> SalvarAsync()
        {
            if (bProcessando)
                return false;

            MensagemErro = "";

            string sTituloDigitado = Titulo?.Trim() ?? "";
            string sModoPreparoDigitado = ModoPreparo?.Trim() ?? "";
            string sObservacoesDigitadas = Observacoes?.Trim() ?? "";

            // ----- Validação do formulário -----
            if (sTituloDigitado.Length < TAMANHO_MINIMO_TITULO || sTituloDigitado.Length > TAMANHO_MAXIMO_TITULO)
            {
                MensagemErro = $"O nome deve ter entre {TAMANHO_MINIMO_TITULO} e {TAMANHO_MAXIMO_TITULO} caracteres.";
                return false;
            }

            if (Ingredientes.Count == 0)
            {
                MensagemErro = "Adicione pelo menos um ingrediente.";
                return false;
            }

            if (sModoPreparoDigitado == "")
            {
                MensagemErro = "Escreva o modo de preparo.";
                return false;
            }

            // ----- Gravação no banco -----
            bProcessando = true;
            try
            {
                ReceitaPersonalizada? adaptacao = await adaptacaoDAO.BuscarPorIdAsync(iIdAdaptacao);
                if (adaptacao == null || adaptacao.IdUsuario != SessaoUsuario.UsuarioAtual?.IdUsuario)
                {
                    MensagemErro = "Esta versão não existe mais.";
                    return false;
                }

                adaptacao.Titulo = sTituloDigitado;
                adaptacao.ModoPreparo = sModoPreparoDigitado;
                adaptacao.Observacoes = sObservacoesDigitadas == "" ? null : sObservacoesDigitadas;

                await adaptacaoDAO.AtualizarAsync(adaptacao, Ingredientes.Select(i => (i.Nome, i.Quantidade, i.Unidade)).ToList());
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível salvar. Tente novamente.";
                return false;
            }
            finally
            {
                bProcessando = false;
            }
        }

        /// <summary>Exclui só a cópia do usuário. Retorna true se excluiu.</summary>
        public async Task<bool> ExcluirAsync()
        {
            if (bProcessando)
                return false;

            MensagemErro = "";
            bProcessando = true;
            try
            {
                await adaptacaoDAO.ExcluirAsync(iIdAdaptacao);
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível excluir. Tente novamente.";
                return false;
            }
            finally
            {
                bProcessando = false;
            }
        }
    }
}
