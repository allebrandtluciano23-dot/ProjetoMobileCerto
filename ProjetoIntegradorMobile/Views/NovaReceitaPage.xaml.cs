using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Tela aberta como MODAL a partir da tela inicial (Navigation.PushModalAsync)
public partial class NovaReceitaPage : ContentPage
{
    // Os dados, a validação e a gravação ficam no ViewModel
    NovaReceitaViewModel vm = new NovaReceitaViewModel();

    public NovaReceitaPage()
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.CarregarCategoriasAsync();
    }

    // Fecha o modal e volta para a tela inicial
    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void AdicionarFoto_Tapped(object sender, TappedEventArgs e)
    {
        await vm.EscolherFotoAsync();
    }

    // RadioButtons de dificuldade (mesmo jeito da aula de RadioButtons)
    private void Dificuldade_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButton? rb = sender as RadioButton;
        if (e.Value && rb != null)
            vm.Dificuldade = rb.Value?.ToString() ?? "";
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

        ToastHelper.Mostrar("Receita salva com sucesso!");
        await Navigation.PopModalAsync();
    }
}
