using ProjetoIntegradorMobile.Helpers;

namespace ProjetoIntegradorMobile.Views;

public partial class PerfilPage : ContentPage
{
    public PerfilPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!SessaoUsuario.EstaLogado)
        {
            await Shell.Current.GoToAsync("//MainPage");
            return;
        }

        lblNome.Text = SessaoUsuario.UsuarioAtual!.Nome;
        lblEmail.Text = SessaoUsuario.UsuarioAtual!.Email;
    }

    // ---------- Ações do perfil ----------
    private async void MinhasReceitas_Tapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Minhas receitas", "Funcionalidade ainda não disponível.", "OK");
    }

    private async void MinhasAdaptacoes_Tapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Minhas adaptações", "Funcionalidade ainda não disponível.", "OK");
    }

    private async void Sair_Tapped(object sender, TappedEventArgs e)
    {
        SessaoUsuario.Encerrar();
        await Shell.Current.GoToAsync("//MainPage");
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

    private void Perfil_Tapped(object sender, TappedEventArgs e)
    {
        // já está na página de perfil
    }
}
