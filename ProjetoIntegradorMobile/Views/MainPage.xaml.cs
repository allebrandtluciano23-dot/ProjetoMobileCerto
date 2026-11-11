using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using ProjetoIntegradorMobile.ViewModels;

namespace ProjetoIntegradorMobile.Views
{
    public partial class MainPage : ContentPage
    {
        // A lista, o filtro e a leitura do banco ficam no ViewModel
        MainViewModel vm = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Recarrega a lista sempre que a tela aparece (ex.: ao voltar depois de criar uma receita)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.CarregarAsync();
        }

        // Usado pelo menu (ex.: depois de sair da conta) para atualizar saudação e favoritos
        public async Task RecarregarAsync()
        {
            await vm.CarregarAsync();
        }

        // Botão de menu (três linhas): abre o menu lateral (Flyout)
        private void Menu_Tapped(object sender, TappedEventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        // ---------- Nova receita ----------

        // Abre a tela Nova receita como MODAL (por cima da tela inicial)
        private async void btNovaReceita_Clicked(object sender, EventArgs e)
        {
            // A receita fica no nome de quem está logado, então precisa entrar antes
            if (!SessaoUsuario.EstaLogado)
            {
                grAvisoLogin.IsVisible = true; // mostra o aviso "Entre na sua conta"
                return;
            }

            NovaReceitaPage pagina = new NovaReceitaPage();

            // Quando o modal fechar, recarrega a lista (para a receita nova aparecer)
            pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

            await Navigation.PushModalAsync(pagina);
        }

        // ---------- Detalhe da receita ----------

        // Abre o detalhe (TabbedPage com abas Resumo | Ingredientes | Preparo) como MODAL
        private async void Receita_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is not ReceitaResumo receita)
                return;

            DetalheReceitaPage pagina = new DetalheReceitaPage(receita.IdReceita);

            // Quando o detalhe fechar, recarrega a lista (a receita pode ter sido editada ou excluída)
            pagina.Disappearing += async (s, ev) => await vm.CarregarAsync();

            await Navigation.PushModalAsync(pagina);
        }

        // ---------- Aviso "Entre na sua conta" ----------

        private async void btAvisoEntrar_Clicked(object sender, EventArgs e)
        {
            grAvisoLogin.IsVisible = false;
            await Shell.Current.GoToAsync(nameof(EntradaPage));
        }

        private void btAvisoCancelar_Clicked(object sender, EventArgs e)
        {
            grAvisoLogin.IsVisible = false;
        }

        private void AvisoFundo_Tapped(object sender, TappedEventArgs e)
        {
            grAvisoLogin.IsVisible = false;
        }

        // ---------- Categorias ----------

        private void Categoria_Tapped(object sender, TappedEventArgs e)
        {
            string sCategoria = e.Parameter as string ?? "Todos";
            vm.CategoriaSelecionada = sCategoria == "Todos" ? "" : sCategoria;

            MarcarCategoria(sender as Border);
        }

        // Destaca o botão da categoria escolhida (cor principal) e volta os outros ao normal
        private void MarcarCategoria(Border? bdSelecionada)
        {
            foreach (var item in slCategorias.Children)
            {
                if (item is Border bd && bd.Content is Label lb)
                {
                    bool bSelecionada = bd == bdSelecionada;
                    bd.BackgroundColor = bSelecionada ? Cor("Destaque") : Cor("Chip");
                    lb.TextColor = bSelecionada ? Colors.White : Cor("Tinta");
                    lb.FontFamily = bSelecionada ? "InterSemiBold" : "Inter";
                }
            }
        }

        // Busca uma cor de Resources/Styles/Colors.xaml pelo nome (ex.: "Destaque")
        private static Color Cor(string sNome)
        {
            if (Application.Current != null
                && Application.Current.Resources.TryGetValue(sNome, out object? valor)
                && valor is Color cor)
                return cor;

            return Colors.Black;
        }

        // ---------- Menu inferior ----------

        private async void Inicio_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void Favoritos_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FavoritosPage));
        }

        private async void Perfil_Tapped(object sender, TappedEventArgs e)
        {
            if (SessaoUsuario.EstaLogado)
                await Shell.Current.GoToAsync(nameof(PerfilPage));
            else
                await Shell.Current.GoToAsync(nameof(EntradaPage));
        }
    }
}
