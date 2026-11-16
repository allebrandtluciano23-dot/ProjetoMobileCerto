using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da Nova receita (também edita)
    public class NovaReceitaViewModel : IngredientesViewModel
    {
        private const int TAMANHO_MINIMO_TITULO = 3;
        private const int TAMANHO_MAXIMO_TITULO = 80;
        private const int MAXIMO_PORCOES = 50;

        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();
        private readonly CategoriaDAO categoriaDAO = new CategoriaDAO();

        // 0 = nova; > 0 = editando
        private readonly int iIdReceitaEdicao;
        private bool bReceitaCarregada = false;

        public NovaReceitaViewModel(int iIdReceita = 0)
        {
            iIdReceitaEdicao = iIdReceita;
        }

        public bool EhEdicao => iIdReceitaEdicao > 0;

        // Textos de criar/editar
        public string TituloTela => EhEdicao ? "Editar receita" : "Nova receita";
        public string TextoBotao => EhEdicao ? "Salvar alterações" : "Salvar receita";

        // Campos da receita

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

        // Categorias do Picker
        private List<string> lCategorias = new List<string>();
        public List<string> Categorias
        {
            get => lCategorias;
            set => SetProperty(ref lCategorias, value);
        }

        private string? sCategoriaSelecionada;
        public string? CategoriaSelecionada
        {
            get => sCategoriaSelecionada;
            set => SetProperty(ref sCategoriaSelecionada, value);
        }

        // Tempo de preparo
        private TimeSpan? tsTempoPreparo = new TimeSpan(0, 30, 0);
        public TimeSpan? TempoPreparo
        {
            get => tsTempoPreparo;
            set => SetProperty(ref tsTempoPreparo, value);
        }

        // Texto, para validar o número
        private string sPorcoes = "";
        public string Porcoes
        {
            get => sPorcoes;
            set => SetProperty(ref sPorcoes, value);
        }

        // Escolhida nos RadioButtons
        private string sDificuldade = "";
        public string Dificuldade
        {
            get => sDificuldade;
            set => SetProperty(ref sDificuldade, value);
        }

        // CheckBox Vegetariana
        private bool bVegetariana = false;
        public bool Vegetariana
        {
            get => bVegetariana;
            set => SetProperty(ref bVegetariana, value);
        }

        // Foto

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

        // Ingredientes: herdados de IngredientesViewModel

        // Ações

        // Carrega as categorias
        public async Task CarregarCategoriasAsync()
        {
            if (Categorias.Count > 0)
                return;

            try
            {
                List<Categoria> lLidas = await categoriaDAO.ListarAsync();
                Categorias = lLidas.Select(c => c.Nome).ToList();
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar as categorias.";
            }
        }

        // Modo edição: preenche a tela; false = não pode editar
        public async Task<bool> CarregarReceitaAsync()
        {
            if (!EhEdicao || bReceitaCarregada)
                return true;

            try
            {
                await CarregarCategoriasAsync();  // Picker precisa das categorias antes

                Receita? receita = await receitaDAO.BuscarPorIdAsync(iIdReceitaEdicao);
                if (receita == null || receita.IdUsuario != SessaoUsuario.UsuarioAtual?.IdUsuario)
                    return false;

                Categoria? categoria = receita.IdCategoria.HasValue
                    ? await categoriaDAO.BuscarPorIdAsync(receita.IdCategoria.Value)
                    : null;

                Titulo = receita.Titulo;
                ModoPreparo = receita.ModoPreparo;
                CategoriaSelecionada = categoria?.Nome;
                TempoPreparo = TimeSpan.FromMinutes(receita.TempoPreparoMin ?? 0);
                Porcoes = receita.Porcoes?.ToString() ?? "";
                Dificuldade = receita.Dificuldade ?? "";
                Vegetariana = receita.Vegetariana;
                CaminhoFoto = receita.CaminhoFoto;

                Ingredientes.Clear();
                foreach (IngredienteDaReceita ingrediente in await receitaDAO.ListarIngredientesAsync(iIdReceitaEdicao))
                    Ingredientes.Add(ingrediente);

                bReceitaCarregada = true;
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar a receita.";
                return true;
            }
        }

        // Escolhe a foto e copia para a pasta do app
        public async Task EscolherFotoAsync()
        {
            try
            {
                string? sNovaFoto = await FotoHelper.EscolherECopiarAsync("fotos");
                if (sNovaFoto != null)  // cancelou
                    CaminhoFoto = sNovaFoto;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar a foto.";
            }
        }

        // Valida e salva (insere ou atualiza)
        public async Task<bool> SalvarAsync()
        {
            if (bProcessando)
                return false;

            MensagemErro = "";

            Usuario? usuario = SessaoUsuario.UsuarioAtual;
            if (usuario == null)
            {
                MensagemErro = "Entre na sua conta para salvar receitas.";
                return false;
            }

            // Validação
            string sTituloDigitado = Titulo?.Trim() ?? "";
            string sModoPreparoDigitado = ModoPreparo?.Trim() ?? "";

            if (sTituloDigitado.Length < TAMANHO_MINIMO_TITULO || sTituloDigitado.Length > TAMANHO_MAXIMO_TITULO)
            {
                MensagemErro = $"O nome da receita deve ter entre {TAMANHO_MINIMO_TITULO} e {TAMANHO_MAXIMO_TITULO} caracteres.";
                return false;
            }

            if (string.IsNullOrEmpty(CategoriaSelecionada))
            {
                MensagemErro = "Escolha a categoria.";
                return false;
            }

            int iTempoMin = (int)(TempoPreparo?.TotalMinutes ?? 0);
            if (iTempoMin <= 0)
            {
                MensagemErro = "Informe o tempo de preparo.";
                return false;
            }

            if (!int.TryParse(Porcoes?.Trim(), out int iPorcoes) || iPorcoes < 1 || iPorcoes > MAXIMO_PORCOES)
            {
                MensagemErro = $"Informe as porções com um número de 1 a {MAXIMO_PORCOES}.";
                return false;
            }

            if (string.IsNullOrEmpty(Dificuldade))
            {
                MensagemErro = "Escolha a dificuldade.";
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
                Categoria? categoria = await categoriaDAO.BuscarPorNomeAsync(CategoriaSelecionada);
                var lIngredientes = Ingredientes.Select(i => (i.Nome, i.Quantidade, i.Unidade)).ToList();

                if (EhEdicao)
                {
                    // Update: mantém autor e data de criação
                    Receita? receita = await receitaDAO.BuscarPorIdAsync(iIdReceitaEdicao);
                    if (receita == null || receita.IdUsuario != usuario.IdUsuario)
                    {
                        MensagemErro = "Só quem criou a receita pode editá-la.";
                        return false;
                    }

                    receita.IdCategoria = categoria?.IdCategoria;
                    receita.Titulo = sTituloDigitado;
                    receita.ModoPreparo = sModoPreparoDigitado;
                    receita.TempoPreparoMin = iTempoMin;
                    receita.Porcoes = iPorcoes;
                    receita.Dificuldade = Dificuldade;
                    receita.Vegetariana = Vegetariana;
                    receita.CaminhoFoto = CaminhoFoto;

                    await receitaDAO.AtualizarAsync(receita, lIngredientes);
                }
                else
                {
                    // Create
                    Receita receita = new Receita
                    {
                        IdUsuario = usuario.IdUsuario,
                        IdCategoria = categoria?.IdCategoria,
                        Titulo = sTituloDigitado,
                        ModoPreparo = sModoPreparoDigitado,
                        TempoPreparoMin = iTempoMin,
                        Porcoes = iPorcoes,
                        Dificuldade = Dificuldade,
                        Vegetariana = Vegetariana,
                        CaminhoFoto = CaminhoFoto
                    };

                    await receitaDAO.InserirAsync(receita, lIngredientes);
                }

                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível salvar a receita. Tente novamente.";
                return false;
            }
            finally
            {
                bProcessando = false;
            }
        }
    }
}
