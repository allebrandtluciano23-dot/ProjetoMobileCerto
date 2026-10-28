using ProjetoIntegradorMobile.Helpers;
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
            foreach (var item in hsCategorias.Children)
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
