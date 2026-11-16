using System.Collections.ObjectModel;
using System.Globalization;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // Lista de ingredientes comum à Nova receita e à Minha versão
    public class IngredientesViewModel : BaseViewModel
    {
        // Lista mostrada na tela
        public ObservableCollection<IngredienteDaReceita> Ingredientes { get; } = new ObservableCollection<IngredienteDaReceita>();

        // Opções de unidade
        public List<string> Unidades { get; } = new List<string>
        {
            "g", "kg", "ml", "l", "un", "colher (sopa)", "colher (chá)", "xícara", "pitada"
        };

        // Campos do novo ingrediente

        private string sNovoIngredienteNome = "";
        public string NovoIngredienteNome
        {
            get => sNovoIngredienteNome;
            set => SetProperty(ref sNovoIngredienteNome, value);
        }

        private string sNovoIngredienteQuantidade = "";
        public string NovoIngredienteQuantidade
        {
            get => sNovoIngredienteQuantidade;
            set => SetProperty(ref sNovoIngredienteQuantidade, value);
        }

        private string? sNovoIngredienteUnidade;
        public string? NovoIngredienteUnidade
        {
            get => sNovoIngredienteUnidade;
            set => SetProperty(ref sNovoIngredienteUnidade, value);
        }

        // Erro dos campos de ingrediente
        private string sErroIngrediente = "";
        public string ErroIngrediente
        {
            get => sErroIngrediente;
            set
            {
                if (SetProperty(ref sErroIngrediente, value))
                    OnPropertyChanged(nameof(TemErroIngrediente));
            }
        }

        public bool TemErroIngrediente => !string.IsNullOrEmpty(ErroIngrediente);

        // Ações

        // Valida e adiciona o ingrediente
        public void AdicionarIngrediente()
        {
            ErroIngrediente = "";

            string sNome = NovoIngredienteNome?.Trim() ?? "";
            string sQuantidade = (NovoIngredienteQuantidade ?? "").Trim().Replace(',', '.');

            if (sNome == "")
            {
                ErroIngrediente = "Digite o nome do ingrediente.";
                return;
            }

            if (!double.TryParse(sQuantidade, NumberStyles.Float, CultureInfo.InvariantCulture, out double dQuantidade)
                || dQuantidade <= 0)
            {
                ErroIngrediente = "Digite uma quantidade válida (ex.: 200 ou 1,5).";
                return;
            }

            if (string.IsNullOrEmpty(NovoIngredienteUnidade))
            {
                ErroIngrediente = "Escolha a unidade.";
                return;
            }

            if (Ingredientes.Any(i => string.Equals(i.Nome, sNome, StringComparison.OrdinalIgnoreCase)))
            {
                ErroIngrediente = "Esse ingrediente já está na lista.";
                return;
            }

            Ingredientes.Add(new IngredienteDaReceita
            {
                Nome = sNome,
                Quantidade = dQuantidade,
                Unidade = NovoIngredienteUnidade
            });

            // Limpa os campos
            NovoIngredienteNome = "";
            NovoIngredienteQuantidade = "";
            NovoIngredienteUnidade = null;
        }

        public void RemoverIngrediente(IngredienteDaReceita ingrediente)
        {
            Ingredientes.Remove(ingrediente);
        }
    }
}
