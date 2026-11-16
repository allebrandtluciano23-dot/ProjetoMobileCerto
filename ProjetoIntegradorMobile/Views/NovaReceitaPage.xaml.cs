using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Nova receita (modal); com id, edita
public partial class NovaReceitaPage : ContentPage
{
    // Lógica no ViewModel
    NovaReceitaViewModel vm;

    public NovaReceitaPage(int iIdReceita = 0)
    {
        InitializeComponent();
        vm = new NovaReceitaViewModel(iIdReceita);
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.CarregarCategoriasAsync();

        if (vm.EhEdicao)
        {
            bool bPodeEditar = await vm.CarregarReceitaAsync();
            if (!bPodeEditar)
            {
                await grAviso.MostrarAsync("Editar receita", "Não foi possível abrir esta receita para edição.", "OK");
                await Navigation.PopModalAsync();
                return;
            }

            MarcarDificuldade(vm.Dificuldade);
        }
    }

    // Edição: marca a dificuldade salva
    private void MarcarDificuldade(string sDificuldade)
    {
        rbFacil.IsChecked = sDificuldade == "Fácil";
        rbMedia.IsChecked = sDificuldade == "Média";
        rbDificil.IsChecked = sDificuldade == "Difícil";
    }

    // Fecha o modal
    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void AdicionarFoto_Tapped(object sender, TappedEventArgs e)
    {
        await vm.EscolherFotoAsync();
    }

    // Guarda a dificuldade escolhida
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

        ToastHelper.Mostrar(vm.EhEdicao ? "Receita atualizada!" : "Receita salva com sucesso!");
        await Navigation.PopModalAsync();
    }
}
