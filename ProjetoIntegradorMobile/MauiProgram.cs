using Microsoft.Extensions.Logging;

namespace ProjetoIntegradorMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    fonts.AddFont("Inter-Regular.otf", "Inter");                    
                    fonts.AddFont("Inter-SemiBold.otf", "InterSemiBold");           
                    fonts.AddFont("BricolageGrotesque-Bold.ttf", "Bricolage");      
                });

            RemoverBordaDosCampos();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RemoverBordaDosCampos()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SemBorda", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 0);
                handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 0);
#endif
            });

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("SemBorda", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 0);
                handler.PlatformView.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 0);
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("SemBorda", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 0);
#endif
            });

            Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("SemBorda", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });
        }
    }
}
