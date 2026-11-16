using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views
{
    public partial class MainPage : ContentPage
    {
        // Lista e filtro ficam no ViewModel
        MainViewModel vm = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Recarrega ao aparecer
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.CarregarAsync();
        }

        // Recarrega a pedido do menu (ex.: ao sair)
        public async Task RecarregarAsync()
        {
            await vm.CarregarAsync();
        }

        // Abre o menu lateral
        private void Menu_Tapped(object sender, TappedEventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        // Nova receita

        // Abre a Nova receita como modal
        private async void btNovaReceita_Clicked(object sender, EventArgs e)
        {
            // Precisa estar logado
            if (!SessaoUsuario.EstaLogado)
            {
                bool bEntrar = await grAviso.MostrarAsync("Entre na sua conta",
                    "Para criar receitas, você precisa entrar na sua conta.",
                    "Entrar", "Agora não");

                if (bEntrar)
                    await Shell.Current.GoToAsync(nameof(EntradaPage));
                return;
            }

            NovaReceitaPage pagina = new NovaReceitaPage();

            // Ao fechar, recarrega a lista
            pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

            await Navigation.PushModalAsync(pagina);
        }

        // Detalhe da receita

        // Abre o detalhe (abas) como modal
        private async void Receita_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not ReceitaResumo receita)
                return;

            DetalheReceitaPage pagina = new DetalheReceitaPage(receita.IdReceita);

            // Ao fechar, recarrega a lista
            pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

            await Navigation.PushModalAsync(pagina);
        }

        // Categorias

        private void Categoria_Tapped(object sender, TappedEventArgs e)
        {
            string sCategoria = e.Parameter as string ?? "Todos";
            vm.CategoriaSelecionada = sCategoria == "Todos" ? "" : sCategoria;

            MarcarCategoria(sender as Border);
        }

        // Destaca a categoria escolhida
        private void MarcarCategoria(Border? bdSelecionada)
        {
            foreach (var item in slCategorias.Children)
            {
                if (item is Border bd && bd.Content is Label lb)
                {
                    bool bSelecionada = bd == bdSelecionada;
                    bd.BackgroundColor = bSelecionada ? CorHelper.Buscar("Destaque") : CorHelper.Buscar("Chip");
                    lb.TextColor = bSelecionada ? Colors.White : CorHelper.Buscar("Tinta");
                    lb.FontFamily = bSelecionada ? "InterSemiBold" : "Inter";
                }
            }
        }

        // Menu inferior

        private async void Inicio_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
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
