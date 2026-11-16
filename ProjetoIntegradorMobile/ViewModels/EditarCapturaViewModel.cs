using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da "Minha versão" (só altera a cópia, nunca a original)
    public class EditarCapturaViewModel : IngredientesViewModel
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

        // Dados da original (só leitura)

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

        // Campos da cópia

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

        private string sObservacoes = "";
        public string Observacoes
        {
            get => sObservacoes;
            set => SetProperty(ref sObservacoes, value);
        }

        // Ingredientes: herdados de IngredientesViewModel

        // Ações

        // Carrega a cópia e a original; false = cópia não existe
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

                // Tempo, porções, dificuldade e foto vêm da original
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

        // Valida e salva a cópia
        public async Task<bool> SalvarAsync()
        {
            if (bProcessando)
                return false;

            MensagemErro = "";

            string sTituloDigitado = Titulo?.Trim() ?? "";
            string sModoPreparoDigitado = ModoPreparo?.Trim() ?? "";
            string sObservacoesDigitadas = Observacoes?.Trim() ?? "";

            // Validação
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

            // Gravação
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

        // Exclui só a cópia
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
