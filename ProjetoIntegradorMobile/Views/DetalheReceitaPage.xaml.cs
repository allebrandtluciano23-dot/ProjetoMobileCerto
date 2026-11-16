using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Detalhe com abas (TabbedPage), aberto como modal
public partial class DetalheReceitaPage : TabbedPage
{
    DetalheReceitaViewModel vm;
    bool bCarregou = false;

    public DetalheReceitaPage(int iIdReceita)
    {
        InitializeComponent();
        vm = new DetalheReceitaViewModel(iIdReceita);
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Carrega só na primeira vez
        if (bCarregou)
            return;
        bCarregou = true;

        bool bExiste = await vm.CarregarAsync();
        if (!bExiste)
        {
            await grAviso.MostrarAsync("Receita", "Esta receita não existe mais.", "OK");
            await Navigation.PopModalAsync();
        }
    }

    private async void btFavoritar_Clicked(object sender, EventArgs e)
    {
        string sMensagem = await vm.AlternarFavoritaAsync();
        if (sMensagem != "")
            ToastHelper.Mostrar(sMensagem);
    }

    // Captura e abre a cópia como modal
    private async void btCapturar_Clicked(object sender, EventArgs e)
    {
        bool bJaTinha = vm.IdAdaptacao > 0;

        int iIdAdaptacao = await vm.CapturarAsync();
        if (iIdAdaptacao == 0)
            return;

        if (!bJaTinha)
            ToastHelper.Mostrar("Receita capturada! Agora edite a sua versão.");

        EditarCapturaPage pagina = new EditarCapturaPage(iIdAdaptacao);

        // Ao voltar, atualiza o botão
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }

    private async void btFechar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    // Abre a Nova receita em modo edição (modal)
    private async void btEditar_Clicked(object sender, EventArgs e)
    {
        NovaReceitaPage pagina = new NovaReceitaPage(vm.IdReceita);

        // Ao fechar, recarrega os dados
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }

    // Abre a confirmação de exclusão (modal)
    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        // Fecha o detalhe depois de excluir
        ExcluirReceitaPage pagina = new ExcluirReceitaPage(vm.IdReceita, vm.Titulo, FecharDepoisDeExcluirAsync);
        await Navigation.PushModalAsync(pagina);
    }

    private async Task FecharDepoisDeExcluirAsync()
    {
        await Navigation.PopModalAsync();
    }
}
