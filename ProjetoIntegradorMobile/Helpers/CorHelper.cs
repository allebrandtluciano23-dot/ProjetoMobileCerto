namespace ProjetoIntegradorMobile.Helpers
{
    // Busca uma cor do Colors.xaml pelo nome
    public static class CorHelper
    {
        public static Color Buscar(string sNome)
        {
            if (Application.Current != null
                && Application.Current.Resources.TryGetValue(sNome, out object? valor)
                && valor is Color cor)
                return cor;

            return Colors.Black;
        }
    }
}
