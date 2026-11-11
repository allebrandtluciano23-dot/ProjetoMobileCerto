using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Receitas que o usuário logado favoritou. Aberta pelo menu Flyout ou pelo menu inferior.
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

    // Abre o detalhe da receita como MODAL; ao fechar, recarrega (a pessoa pode ter desfavoritado)
    private async void Receita_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not ReceitaResumo receita)
            return;

        DetalheReceitaPage pagina = new DetalheReceitaPage(receita.IdReceita);
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }
}
