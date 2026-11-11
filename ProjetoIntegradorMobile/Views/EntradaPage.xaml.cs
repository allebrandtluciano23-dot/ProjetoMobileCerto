using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

public partial class EntradaPage : ContentPage
{
    // Os dados e a lógica do login ficam no ViewModel; aqui ficam só eventos de tela e navegação
    EntradaViewModel vm = new EntradaViewModel();

    public EntradaPage()
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void MostrarSenha_Tapped(object sender, TappedEventArgs e)
    {
        txSenha.IsPassword = !txSenha.IsPassword;
        imOlho.Source = txSenha.IsPassword ? "eye_off.png" : "eye.png";
    }

    private async void EsqueceuSenha_Tapped(object sender, TappedEventArgs e)
    {
        await grAviso.MostrarAsync("Esqueceu sua senha?", "Esta função ainda não está disponível.", "OK");
    }

    private async void Entrar_Tapped(object sender, TappedEventArgs e)
    {
        bool bSucesso = await vm.EntrarAsync();
        if (bSucesso)
            await Shell.Current.GoToAsync("//MainPage");
    }

    private async void Cadastrar_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroPage));
    }
}
