
namespace ProjetoIntegradorMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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