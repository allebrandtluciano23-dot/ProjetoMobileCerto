using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views;

// Detalhe da receita com ABAS (TabbedPage). Aberta como MODAL pela tela inicial.
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

        // Carrega só na primeira vez; depois de editar, quem recarrega é o btEditar_Clicked
        if (bCarregou)
            return;
        bCarregou = true;

        bool bExiste = await vm.CarregarAsync();
        if (!bExiste)
        {
            await DisplayAlertAsync("Receita", "Esta receita não existe mais.", "OK");
            await Navigation.PopModalAsync();
        }
    }

    private async void btFavoritar_Clicked(object sender, EventArgs e)
    {
        string sMensagem = await vm.AlternarFavoritaAsync();
        if (sMensagem != "")
            ToastHelper.Mostrar(sMensagem);
    }

    // Captura a receita (se ainda não capturou) e abre "Minha versão" como MODAL
    private async void btCapturar_Clicked(object sender, EventArgs e)
    {
        bool bJaTinha = vm.IdAdaptacao > 0;

        int iIdAdaptacao = await vm.CapturarAsync();
        if (iIdAdaptacao == 0)
            return;

        if (!bJaTinha)
            ToastHelper.Mostrar("Receita capturada! Agora edite a sua versão.");

        EditarCapturaPage pagina = new EditarCapturaPage(iIdAdaptacao);

        // Ao voltar, atualiza o botão (a versão pode ter sido excluída)
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }

    private async void btFechar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    // Abre a mesma tela de Nova receita, mas em modo de edição (também como Modal)
    private async void btEditar_Clicked(object sender, EventArgs e)
    {
        NovaReceitaPage pagina = new NovaReceitaPage(vm.IdReceita);

        // Quando a edição fechar, mostra os dados atualizados
        pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

        await Navigation.PushModalAsync(pagina);
    }

    // Abre a tela de confirmação de exclusão (também como Modal)
    private async void btExcluir_Clicked(object sender, EventArgs e)
    {
        // Se a receita for excluída, a tela de confirmação chama esta função para fechar o detalhe também
        ExcluirReceitaPage pagina = new ExcluirReceitaPage(vm.IdReceita, vm.Titulo, FecharDepoisDeExcluirAsync);
        await Navigation.PushModalAsync(pagina);
    }

    private async Task FecharDepoisDeExcluirAsync()
    {
        await Navigation.PopModalAsync();
    }
}
