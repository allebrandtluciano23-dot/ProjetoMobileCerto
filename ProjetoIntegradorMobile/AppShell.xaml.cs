using ProjetoIntegradorMobile.Views;

namespace ProjetoIntegradorMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(BuscarPage), typeof(BuscarPage));
            Routing.RegisterRoute(nameof(FavoritosPage), typeof(FavoritosPage));
            Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
        }
    }
}
