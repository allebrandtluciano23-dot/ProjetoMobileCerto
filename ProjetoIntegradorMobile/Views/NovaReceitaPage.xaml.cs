namespace ProjetoIntegradorMobile.Views;

public partial class NovaReceitaPage : ContentPage
{
    private readonly List<string> _ingredientes = new();
    private string? _caminhoFoto;

    public NovaReceitaPage()
    {
        InitializeComponent();

        categoriaPicker.ItemsSource = new List<string>
        {
            "Rápidos",
            "Almoço",
            "Jantar",
            "Sobremesas"
        };
    }

    private async void Voltar_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void AdicionarFoto_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void AdicionarIngrediente_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void RemoverIngrediente_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void AtualizarListaIngredientes()
    {
        
    }

    private async void Salvar_Tapped(object sender, TappedEventArgs e)
    {
       
    }

    private void MostrarErro(string mensagem)
    {
    }