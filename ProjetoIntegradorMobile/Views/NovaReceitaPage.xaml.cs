using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Tela aberta como MODAL (Navigation.PushModalAsync).
// new NovaReceitaPage()   -> cria uma receita nova (tela inicial)
// new NovaReceitaPage(id) -> edita a receita com esse id (tela de detalhe)
public partial class NovaReceitaPage : ContentPage
{
    // Os dados, a validação e a gravação ficam no ViewModel
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
                await DisplayAlertAsync("Editar receita", "Não foi possível abrir esta receita para edição.", "OK");
                await Navigation.PopModalAsync();
                return;
            }

            MarcarDificuldade(vm.Dificuldade);
        }
    }

    // Modo edição: marca o RadioButton da dificuldade que veio do banco
    private void MarcarDificuldade(string sDificuldade)
    {
        rbFacil.IsChecked = sDificuldade == "Fácil";
        rbMedia.IsChecked = sDificuldade == "Média";
        rbDificil.IsChecked = sDificuldade == "Difícil";
    }

    // Fecha o modal e volta para a tela anterior
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

        ToastHelper.Mostrar(vm.EhEdicao ? "Receita atualizada!" : "Receita salva com sucesso!");
        await Navigation.PopModalAsync();
    }
}
