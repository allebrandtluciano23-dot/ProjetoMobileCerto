using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Confirmação de exclusão (modal)
public partial class ExcluirReceitaPage : ContentPage
{
    ExcluirReceitaViewModel vm;

    // Fecha o detalhe depois de excluir
    readonly Func<Task>? aoExcluir;

    bool bVerificouCapturas = false;

    public ExcluirReceitaPage(int iIdReceita, string sNomeReceita, Func<Task>? aoExcluir = null)
    {
        InitializeComponent();
        vm = new ExcluirReceitaViewModel(iIdReceita, sNomeReceita);
        BindingContext = vm;
        this.aoExcluir = aoExcluir;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Avisa se alguém capturou
        if (!bVerificouCapturas)
        {
            bVerificouCapturas = true;
            await vm.VerificarCapturasAsync();
        }
    }

    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void btCancelar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        bool bSucesso = await vm.ExcluirAsync();
        if (!bSucesso)
            return;

        ToastHelper.Mostrar("Receita excluída.");

        await Navigation.PopModalAsync();  // fecha a confirmação

        if (aoExcluir != null)
            await aoExcluir();  // fecha o detalhe
    }
}
