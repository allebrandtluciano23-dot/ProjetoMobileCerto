using Microsoft.Maui.Controls.Shapes;
using ProjetoIntegradorMobile.Helpers;

namespace ProjetoIntegradorMobile.Views;

// Cartão de aviso/confirmação usado no lugar do DisplayAlertAsync.
// Cada página que precisa dele coloca <views:CartaoAviso x:Name="grAviso"/> por cima do conteúdo.
public partial class CartaoAviso : Grid
{
    // Guarda a resposta até o usuário tocar em um dos botões
    TaskCompletionSource<bool>? tcsResposta;

    public CartaoAviso()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Mostra o cartão e espera a resposta.
    /// Retorna true se tocou no botão de confirmar; false se cancelou ou tocou fora.
    /// Sem sTextoCancelar, aparece só um botão (aviso simples, tipo "OK").
    /// bPerigo = true: ícone de lixeira e botão vermelho (exclusões).
    /// </summary>
    public Task<bool> MostrarAsync(string sTitulo, string sMensagem, string sTextoConfirmar,
                                   string? sTextoCancelar = null, bool bPerigo = false)
    {
        lbTitulo.Text = sTitulo;
        lbMensagem.Text = sMensagem;

        btConfirmar.Text = sTextoConfirmar;
        btConfirmar.BackgroundColor = CorHelper.Buscar(bPerigo ? "Perigo" : "Destaque");

        btCancelar.Text = sTextoCancelar ?? "";
        btCancelar.IsVisible = sTextoCancelar != null;

        bdIcone.BackgroundColor = CorHelper.Buscar(bPerigo ? "PerigoClaro" : "FundoAvatar");
        paIcone.Stroke = CorHelper.Buscar(bPerigo ? "Perigo" : "Destaque");
        if (Application.Current != null
            && Application.Current.Resources.TryGetValue(bPerigo ? "IconeLixeira" : "IconeChapeu", out object? icone)
            && icone is Geometry desenho)
            paIcone.Data = desenho;

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
