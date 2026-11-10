using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views
{
    public partial class MainPage : ContentPage
    {
        // A lista, o filtro e a leitura do banco ficam no ViewModel
        MainViewModel vm = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Recarrega a lista sempre que a tela aparece (ex.: ao voltar depois de criar uma receita)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.CarregarAsync();
        }

        // ---------- Nova receita ----------

        // Abre a tela Nova receita como MODAL (por cima da tela inicial)
        private async void btNovaReceita_Clicked(object sender, EventArgs e)
        {
            // A receita fica no nome de quem está logado, então precisa entrar antes
            if (!SessaoUsuario.EstaLogado)
            {
                bool bEntrar = await DisplayAlertAsync("Entrar",
                    "Para criar receitas, entre na sua conta.", "Entrar", "Cancelar");
                if (bEntrar)
                    await Shell.Current.GoToAsync(nameof(EntradaPage));
                return;
            }

            NovaReceitaPage pagina = new NovaReceitaPage();

            // Quando o modal fechar, recarrega a lista (para a receita nova aparecer)
            pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

            await Navigation.PushModalAsync(pagina);
        }

        // ---------- Detalhe da receita ----------

        // Abre o detalhe (TabbedPage com abas Resumo | Ingredientes | Preparo) como MODAL
        private async void Receita_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not ReceitaResumo receita)
                return;

            DetalheReceitaPage pagina = new DetalheReceitaPage(receita.IdReceita);

            // Quando o detalhe fechar, recarrega a lista (a receita pode ter sido editada ou excluída)
            pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

            await Navigation.PushModalAsync(pagina);
        }

        // ---------- Categorias ----------

        private void Categoria_Tapped(object sender, TappedEventArgs e)
        {
            string sCategoria = e.Parameter as string ?? "Todos";
            vm.CategoriaSelecionada = sCategoria == "Todos" ? "" : sCategoria;

            MarcarCategoria(sender as Border);
        }

        // Destaca o botão da categoria escolhida (fundo preto) e volta os outros ao normal
        private void MarcarCategoria(Border? bdSelecionada)
        {
            foreach (var item in slCategorias.Children)
            {
                if (item is Border bd && bd.Content is Label lb)
                {
                    bool bSelecionada = bd == bdSelecionada;
                    bd.BackgroundColor = bSelecionada ? Colors.Black : Color.FromArgb("#F3F3F3");
                    lb.TextColor = bSelecionada ? Colors.White : Colors.Black;
                    lb.FontAttributes = bSelecionada ? FontAttributes.Bold : FontAttributes.None;
                }
            }
        }

        // ---------- Menu inferior ----------

        private async void Inicio_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void Buscar_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(BuscarPage));
        }

        private async void Favoritos_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FavoritosPage));
        }

        private async void Perfil_Tapped(object sender, TappedEventArgs e)
        {
            if (SessaoUsuario.EstaLogado)
                await Shell.Current.GoToAsync(nameof(PerfilPage));
            else
                await Shell.Current.GoToAsync(nameof(EntradaPage));
        }
    }
}
