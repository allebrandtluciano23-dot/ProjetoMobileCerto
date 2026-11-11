using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da tela Nova receita (NovaReceitaPage).
    // A mesma tela serve para EDITAR: basta criar com o id da receita.
    public class NovaReceitaViewModel : IngredientesViewModel
    {
        private const int TAMANHO_MINIMO_TITULO = 3;
        private const int TAMANHO_MAXIMO_TITULO = 80;
        private const int MAXIMO_PORCOES = 50;

        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();
        private readonly CategoriaDAO categoriaDAO = new CategoriaDAO();

        // 0 = receita nova; maior que 0 = editando a receita com esse id
        private readonly int iIdReceitaEdicao;
        private bool bReceitaCarregada = false;

        public NovaReceitaViewModel(int iIdReceita = 0)
        {
            iIdReceitaEdicao = iIdReceita;
        }

        public bool EhEdicao => iIdReceitaEdicao > 0;

        // Textos que mudam entre criar e editar
        public string TituloTela => EhEdicao ? "Editar receita" : "Nova receita";
        public string TextoBotao => EhEdicao ? "Salvar alterações" : "Salvar receita";

        // ---------- Campos da receita (ligados por Binding) ----------

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

        // Nomes das categorias para o Picker (lidos do banco)
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

        // TimePicker: horas e minutos de preparo (00:30 = 30 minutos)
        private TimeSpan? tsTempoPreparo = new TimeSpan(0, 30, 0);
        public TimeSpan? TempoPreparo
        {
            get => tsTempoPreparo;
            set => SetProperty(ref tsTempoPreparo, value);
        }

        // Fica como texto para conseguir validar se é um número
        private string sPorcoes = "";
        public string Porcoes
        {
            get => sPorcoes;
            set => SetProperty(ref sPorcoes, value);
        }

        // Preenchida pelos RadioButtons (Fácil, Média, Difícil)
        private string sDificuldade = "";
        public string Dificuldade
        {
            get => sDificuldade;
            set => SetProperty(ref sDificuldade, value);
        }

        // CheckBox "Vegetariana" (campo booleano da tabela receita)
        private bool bVegetariana = false;
        public bool Vegetariana
        {
            get => bVegetariana;
            set => SetProperty(ref bVegetariana, value);
        }

        // ---------- Foto ----------

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

        // Ingredientes (lista, campos, adicionar e remover): herdados de IngredientesViewModel

        // ---------- Ações ----------

        /// <summary>Lê as categorias do banco para o Picker (só na primeira vez).</summary>
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

        /// <summary>
        /// Modo edição: preenche a tela com os dados da receita (só na primeira vez).
        /// Retorna false se a receita não existe mais ou se o usuário não é o autor.
        /// </summary>
        public async Task<bool> CarregarReceitaAsync()
        {
            if (!EhEdicao || bReceitaCarregada)
                return true;

            try
            {
                await CarregarCategoriasAsync(); // o Picker precisa das categorias antes de marcar a escolhida

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

        /// <summary>
        /// Abre a galeria. A foto escolhida é copiada para a pasta do app,
        /// assim ela continua aparecendo mesmo se for apagada da galeria.
        /// </summary>
        public async Task EscolherFotoAsync()
        {
            try
            {
                string? sNovaFoto = await FotoHelper.EscolherECopiarAsync("fotos");
                if (sNovaFoto != null) // null = usuário cancelou
                    CaminhoFoto = sNovaFoto;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível carregar a foto.";
            }
        }

        /// <summary>
        /// Valida o formulário e grava a receita com os ingredientes
        /// (insere se for nova, atualiza se estiver editando).
        /// Retorna true se salvou (a tela então mostra o Toast e fecha).
        /// </summary>
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

            // ----- Validação do formulário -----
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

            // ----- Gravação no banco -----
            bProcessando = true;
            try
            {
                Categoria? categoria = await categoriaDAO.BuscarPorNomeAsync(CategoriaSelecionada);
                var lIngredientes = Ingredientes.Select(i => (i.Nome, i.Quantidade, i.Unidade)).ToList();

                if (EhEdicao)
                {
                    // UPDATE: parte da receita salva (mantém autor e data de criação) e troca os campos
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
                    // CREATE
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
