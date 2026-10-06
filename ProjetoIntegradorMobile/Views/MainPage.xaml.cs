using ProjetoIntegradorMobile.Views;

namespace ProjetoIntegradorMobile.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
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
            await Shell.Current.GoToAsync(nameof(PerfilPage));
        }
    }
}
