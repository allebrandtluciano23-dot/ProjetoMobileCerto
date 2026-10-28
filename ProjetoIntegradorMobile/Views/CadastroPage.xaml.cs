using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

public partial class CadastroPage : ContentPage
{
    // Os dados, a validação e o cadastro ficam no ViewModel; aqui ficam só eventos de tela e navegação
    CadastroViewModel vm = new CadastroViewModel();

    public CadastroPage()
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void MostrarSenha_Tapped(object sender, TappedEventArgs e)
    {
        senhaEntry.IsPassword = !senhaEntry.IsPassword;
        imgOlho.Source = senhaEntry.IsPassword ? "eye_off.png" : "eye.png";
    }

    private async void Cadastrar_Tapped(object sender, TappedEventArgs e)
    {
        bool bSucesso = await vm.CadastrarAsync();
        if (bSucesso)
            await Shell.Current.GoToAsync("//MainPage");
    }

    private async void Entrar_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EntradaPage));
    }
}
