namespace ProjetoIntegradorMobile.Helpers
{
    // Toast nativo do Android (no Windows não aparece)
    public static class ToastHelper
    {
        public static void Mostrar(string sMensagem)
        {
#if ANDROID
            try
            {
                Android.Widget.Toast.MakeText(
                    Android.App.Application.Context,
                    sMensagem,
                    Android.Widget.ToastLength.Short)?.Show();
            }
            catch (Exception)
            {
                // Falha no Toast não afeta a ação
            }
#endif
        }
    }
}
