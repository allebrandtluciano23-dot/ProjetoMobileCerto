using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Receitas favoritas do usuário
public partial class FavoritosPage : ContentPage
{
    ListaReceitasViewModel vm = new ListaReceitasViewModel(TipoLista.Favoritas);

    public FavoritosPage()
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
}
