using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

public partial class CadastroPage : ContentPage
{
    // Lógica no ViewModel; aqui só eventos e navegação
    CadastroViewModel vm = new CadastroViewModel();

    public CadastroPage()
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // Volta para o login (PopAsync)
    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void MostrarSenha_Tapped(object sender, TappedEventArgs e)
    {
        txSenha.IsPassword = !txSenha.IsPassword;
        imOlho.Source = txSenha.IsPassword ? "eye_off.png" : "eye.png";
    }

    private async void Cadastrar_Tapped(object sender, TappedEventArgs e)
    {
        bool bSucesso = await vm.CadastrarAsync();
        if (bSucesso)
            await Shell.Current.GoToAsync("//MainPage");
    }

    // Volta para o login (já está na pilha)
    private async void Entrar_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}
