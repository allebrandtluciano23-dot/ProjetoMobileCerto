using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Views;

namespace ProjetoIntegradorMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(FavoritosPage), typeof(FavoritosPage));
            Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
            Routing.RegisterRoute(nameof(EntradaPage), typeof(EntradaPage));
            Routing.RegisterRoute(nameof(CadastroPage), typeof(CadastroPage));
            Routing.RegisterRoute(nameof(CapturasPage), typeof(CapturasPage));
            Routing.RegisterRoute(nameof(ReceitasCriadasPage), typeof(ReceitasCriadasPage));
        }

        // Sempre que o menu abre, mostra quem está logado e troca "Entrar" / "Sair"
        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(FlyoutIsPresented) && FlyoutIsPresented)
                AtualizarMenu();
        }

        private void AtualizarMenu()
        {
            if (SessaoUsuario.UsuarioAtual != null)
            {
                lbMenuNome.Text = SessaoUsuario.UsuarioAtual.Nome;
                lbMenuEmail.Text = SessaoUsuario.UsuarioAtual.Email;
                miConta.Text = "Sair da conta";
                MostrarFoto(SessaoUsuario.UsuarioAtual.CaminhoFoto);
            }
            else
            {
                lbMenuNome.Text = "CheefBook";
                lbMenuEmail.Text = "Entre para criar, favoritar e capturar receitas.";
                miConta.Text = "Entrar";
                MostrarFoto(null);
            }
        }

        // Foto do perfil no topo do menu (sem foto: mostra o desenho do chapéu de chef)
        private void MostrarFoto(string? sCaminhoFoto)
        {
            bool bTemFoto = !string.IsNullOrEmpty(sCaminhoFoto);
            imMenuFoto.Source = bTemFoto ? ImageSource.FromFile(sCaminhoFoto) : null;
            imMenuFoto.IsVisible = bTemFoto;
            grMenuSemFoto.IsVisible = !bTemFoto;
        }

        // Fecha o menu e abre a tela. As que dependem de login mandam para o login se ninguém entrou.
        private async Task AbrirAsync(string sRota, bool bPrecisaLogin)
        {
            FlyoutIsPresented = false;

            if (bPrecisaLogin && !SessaoUsuario.EstaLogado)
            {
                ToastHelper.Mostrar("Entre na sua conta para continuar.");
                await GoToAsync(nameof(EntradaPage));
                return;
            }

            await GoToAsync(sRota);
        }

        // ---------- Opções do menu Flyout ----------

        private async void miInicio_Clicked(object sender, EventArgs e)
        {
            await AbrirAsync("//MainPage", false);
        }

        private async void miFavoritos_Clicked(object sender, EventArgs e)
        {
            await AbrirAsync(nameof(FavoritosPage), true);
        }

        private async void miMinhasReceitas_Clicked(object sender, EventArgs e)
        {
            await AbrirAsync(nameof(ReceitasCriadasPage), true);
        }

        private async void miCapturas_Clicked(object sender, EventArgs e)
        {
            await AbrirAsync(nameof(CapturasPage), true);
        }

        private async void miPerfil_Clicked(object sender, EventArgs e)
        {
            await AbrirAsync(nameof(PerfilPage), true);
        }

        // "Entrar" se ninguém está logado; "Sair da conta" se alguém está
        private async void miConta_Clicked(object sender, EventArgs e)
        {
            if (!SessaoUsuario.EstaLogado)
            {
                await AbrirAsync(nameof(EntradaPage), false);
                return;
            }

            FlyoutIsPresented = false;
            SessaoUsuario.Encerrar();
            ToastHelper.Mostrar("Você saiu da conta.");
            await GoToAsync("//MainPage");

            // Se já estava na tela inicial, ela não "reaparece": atualiza saudação e favoritos na mão
            if (CurrentPage is MainPage paginaInicial)
                await paginaInicial.RecarregarAsync();
        }
    }
}
