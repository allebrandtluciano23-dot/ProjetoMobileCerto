using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Confirmação de exclusão. Aberta como MODAL pela tela de detalhe da receita.
public partial class ExcluirReceitaPage : ContentPage
{
    ExcluirReceitaViewModel vm;

    // Função da tela de detalhe, chamada depois de excluir (para o detalhe fechar também)
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

        // Mostra o aviso se alguém já capturou a receita
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

        await Navigation.PopModalAsync();   // fecha esta confirmação

        if (aoExcluir != null)
            await aoExcluir();              // fecha a tela de detalhe também
    }
}
