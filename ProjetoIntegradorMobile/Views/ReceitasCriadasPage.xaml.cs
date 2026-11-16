using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Receitas criadas pelo usuário
public partial class ReceitasCriadasPage : ContentPage
{
    ListaReceitasViewModel vm = new ListaReceitasViewModel(TipoLista.CriadasPorMim);

    public ReceitasCriadasPage()
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.CarregarAsync();
    }

    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // Abre o detalhe como modal; ao fechar, recarrega
    private async void Receita_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not ReceitaResumo receita)
            return;

        DetalheReceitaPage pagina = new DetalheReceitaPage(receita.IdReceita);
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }

    // Abre a Nova receita como modal; ao fechar, recarrega
    private async void btNovaReceita_Clicked(object sender, EventArgs e)
    {
        if (!SessaoUsuario.EstaLogado)
        {
            await Shell.Current.GoToAsync(nameof(EntradaPage));
            return;
        }

        NovaReceitaPage pagina = new NovaReceitaPage();
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }
}
