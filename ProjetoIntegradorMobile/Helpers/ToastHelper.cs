namespace ProjetoIntegradorMobile.Helpers
{
    /// <summary>
    /// Toast: mensagem rápida que aparece no rodapé da tela e some sozinha.
    /// Usa o Toast nativo do Android (não precisa de nenhum pacote NuGet).
    /// Em outras plataformas (ex.: Windows) não mostra nada.
    /// </summary>
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
                // Se o Toast falhar, a ação já foi feita: só não mostra a mensagem
            }
#endif
        }
    }
}
