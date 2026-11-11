using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// "Minha versão" da receita capturada. Aberta como MODAL.
public partial class EditarCapturaPage : ContentPage
{
    EditarCapturaViewModel vm;

    public EditarCapturaPage(int iIdAdaptacao)
    {
        InitializeComponent();
        vm = new EditarCapturaViewModel(iIdAdaptacao);
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool bExiste = await vm.CarregarAsync();
        if (!bExiste)
        {
            await grAviso.MostrarAsync("Minha versão", "Esta versão não existe mais.", "OK");
            await Navigation.PopModalAsync();
        }
    }

    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void btAdicionarIngrediente_Clicked(object sender, EventArgs e)
    {
        vm.AdicionarIngrediente();
    }

    private void RemoverIngrediente_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is IngredienteDaReceita ingrediente)
            vm.RemoverIngrediente(ingrediente);
    }

    private async void btSalvar_Clicked(object sender, EventArgs e)
    {
        bool bSucesso = await vm.SalvarAsync();
        if (!bSucesso)
            return;

        ToastHelper.Mostrar("Sua versão foi salva!");
        await Navigation.PopModalAsync();
    }

    // Confirmação antes de excluir (requisito: confirmação de exclusão)
    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        bool bConfirmou = await grAviso.MostrarAsync("Excluir minha versão?",
            "Sua cópia e suas mudanças serão apagadas. A receita original não é afetada.",
            "Excluir", "Cancelar", bPerigo: true);
        if (!bConfirmou)
            return;

        bool bSucesso = await vm.ExcluirAsync();
        if (!bSucesso)
            return;

        ToastHelper.Mostrar("Sua versão foi excluída.");
        await Navigation.PopModalAsync();
    }
}
