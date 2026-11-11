
using ProjetoIntegradorMobile.Views;

namespace ProjetoIntegradorMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // O visual do app foi feito para o modo claro: mesmo com o celular no modo escuro,
            // as telas continuam com o fundo creme
            UserAppTheme = AppTheme.Light;

            InicializarBanco();

        }

        private async void InicializarBanco()
        {
            await DatabaseConfig.Database.Inicializar();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}