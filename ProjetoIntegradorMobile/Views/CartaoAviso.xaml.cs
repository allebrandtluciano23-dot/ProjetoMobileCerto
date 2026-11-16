using ProjetoIntegradorMobile.Helpers;

namespace ProjetoIntegradorMobile.Views;

// Cartão de aviso/confirmação (substitui o DisplayAlertAsync)
public partial class CartaoAviso : Grid
{
    // Resposta do usuário
    TaskCompletionSource<bool>? tcsResposta;

    public CartaoAviso()
    {
        InitializeComponent();
    }

    // Mostra o cartão; true = confirmou. bPerigo = lixeira e botão vermelho
    public Task<bool> MostrarAsync(string sTitulo, string sMensagem, string sTextoConfirmar,
                                   string? sTextoCancelar = null, bool bPerigo = false)
    {
        lbTitulo.Text = sTitulo;
        lbMensagem.Text = sMensagem;

        btConfirmar.Text = sTextoConfirmar;
        btConfirmar.BackgroundColor = CorHelper.Buscar(bPerigo ? "Perigo" : "Destaque");

        btCancelar.Text = sTextoCancelar ?? "";
        btCancelar.IsVisible = sTextoCancelar != null;

        // Exclusão: lixeira; aviso comum: chef
        bdIcone.BackgroundColor = CorHelper.Buscar(bPerigo ? "PerigoClaro" : "FundoAvatar");
        paIcone.IsVisible = bPerigo;
        imIcone.IsVisible = !bPerigo;

        tcsResposta = new TaskCompletionSource<bool>();
        IsVisible = true;
        return tcsResposta.Task;
    }

    private void Responder(bool bConfirmou)
    {
        IsVisible = false;
        tcsResposta?.TrySetResult(bConfirmou);
        tcsResposta = null;
    }

    private void btConfirmar_Clicked(object sender, EventArgs e)
    {
        Responder(true);
    }

    private void btCancelar_Clicked(object sender, EventArgs e)
    {
        Responder(false);
    }

    private void Fundo_Tapped(object sender, TappedEventArgs e)
    {
        Responder(false);
    }
}
