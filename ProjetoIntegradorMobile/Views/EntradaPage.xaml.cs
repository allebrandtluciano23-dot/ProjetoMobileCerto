using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;

namespace ProjetoIntegradorMobile.Views;

public partial class EntradaPage : ContentPage
{
    UsuarioDAO usuarioDAO = new UsuarioDAO();

    public EntradaPage()
    {
        InitializeComponent();
    }

    private void MostrarSenha_Tapped(object sender, TappedEventArgs e)
    {
        senhaEntry.IsPassword = !senhaEntry.IsPassword;
        imgOlho.Source = senhaEntry.IsPassword ? "eye_off.png" : "eye.png";
    }

    private async void EsqueceuSenha_Tapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Esqueceu sua senha", "Funcionalidade ainda não disponível.", "OK");
    }

    private async void Entrar_Tapped(object sender, TappedEventArgs e)
    {
        lblErro.IsVisible = false;

        var email = usuarioEntry.Text?.Trim() ?? "";
        var senha = senhaEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            MostrarErro("Preencha usuário e senha.");
            return;
        }

        var usuario = await usuarioDAO.AutenticarAsync(email, senha);
        if (usuario == null)
        {
            MostrarErro("E-mail ou senha inválidos.");
            return;
        }

        SessaoUsuario.UsuarioAtual = usuario;
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void Cadastrar_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroPage));
    }

    private void MostrarErro(string mensagem)
    {
        lblErro.Text = mensagem;
        lblErro.IsVisible = true;
    }
}
