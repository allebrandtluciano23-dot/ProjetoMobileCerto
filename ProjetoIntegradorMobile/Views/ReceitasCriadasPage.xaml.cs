using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// "Minhas receitas": receitas criadas pelo usuário logado. Aberta pelo menu Flyout ou pelo Perfil.
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

    // Abre o detalhe da receita como MODAL; ao fechar, recarrega (pode ter sido editada ou excluída)
    private async void Receita_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not ReceitaResumo receita)
            return;

        DetalheReceitaPage pagina = new DetalheReceitaPage(receita.IdReceita);
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }

    // Abre a tela Nova receita como MODAL; ao fechar, recarrega a lista
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
