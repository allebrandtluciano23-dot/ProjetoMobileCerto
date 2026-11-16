using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

public partial class PerfilPage : ContentPage
{
    // Dados no ViewModel
    PerfilViewModel vm = new PerfilViewModel();

    public PerfilPage()
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!SessaoUsuario.EstaLogado)
        {
            await Shell.Current.GoToAsync("//MainPage");
            return;
        }

        vm.Carregar();
    }

    // Troca a foto do perfil
    private async void Foto_Tapped(object sender, TappedEventArgs e)
    {
        bool bTrocou = await vm.AlterarFotoAsync();
        if (bTrocou)
            ToastHelper.Mostrar("Foto do perfil atualizada!");
    }

    // Atalhos do perfil
    private async void MinhasReceitas_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ReceitasCriadasPage));
    }

    private async void MinhasAdaptacoes_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CapturasPage));
    }

    private async void Sair_Tapped(object sender, TappedEventArgs e)
    {
        SessaoUsuario.Encerrar();
        await Shell.Current.GoToAsync("//MainPage");
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

    private void Perfil_Tapped(object sender, TappedEventArgs e)
    {
        // já está no perfil
    }
}
