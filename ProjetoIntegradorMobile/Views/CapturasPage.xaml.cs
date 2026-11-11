using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Lista "Minhas adaptações". Aberta pelo Perfil com Shell.Current.GoToAsync (navegação em pilha).
public partial class CapturasPage : ContentPage
{
    CapturasViewModel vm = new CapturasViewModel();

    public CapturasPage()
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!SessaoUsuario.EstaLogado)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        await vm.CarregarAsync();
    }

    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // Abre "Minha versão" como MODAL; ao voltar, recarrega a lista (pode ter sido editada ou excluída)
    private async void Adaptacao_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not AdaptacaoResumo adaptacao)
            return;

        EditarCapturaPage pagina = new EditarCapturaPage(adaptacao.IdReceitaPersonalizada);
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }
}
