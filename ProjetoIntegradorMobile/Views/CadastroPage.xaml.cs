using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;

namespace ProjetoIntegradorMobile.Views;

public partial class CadastroPage : ContentPage
{
    UsuarioDAO usuarioDAO = new UsuarioDAO();

    public CadastroPage()
    {
        InitializeComponent();
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
        lblErro.IsVisible = false;

        var nome = nomeEntry.Text?.Trim() ?? "";
        var email = emailEntry.Text?.Trim() ?? "";
        var senha = senhaEntry.Text ?? "";
        var confirmarSenha = confirmarSenhaEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            MostrarErro("Preencha todos os campos.");
            return;
        }

        if (senha != confirmarSenha)
        {
            MostrarErro("As senhas não coincidem.");
            return;
        }

        if (senha.Length < 6)
        {
            MostrarErro("A senha deve ter pelo menos 6 caracteres.");
            return;
        }

        var usuario = await usuarioDAO.CadastrarAsync(nome, email, senha);
        if (usuario == null)
        {
            MostrarErro("Já existe uma conta com esse e-mail.");
            return;
        }

        SessaoUsuario.UsuarioAtual = usuario;
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void Entrar_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(EntradaPage));
    }

    private void MostrarErro(string mensagem)
    {
        lblErro.Text = mensagem;
        lblErro.IsVisible = true;
    }
}
