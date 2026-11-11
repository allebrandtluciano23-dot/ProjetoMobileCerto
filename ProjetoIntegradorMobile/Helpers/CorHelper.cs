namespace ProjetoIntegradorMobile.Helpers
{
    // Busca uma cor de Resources/Styles/Colors.xaml pelo nome (ex.: "Destaque"),
    // para quando o código (e não o XAML) precisa trocar uma cor.
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
