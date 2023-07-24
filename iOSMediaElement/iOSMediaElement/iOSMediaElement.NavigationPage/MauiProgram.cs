using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace iOSMediaElement.NavigationPage
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		    builder.Logging.AddDebug();
#endif

#if IOS || MACCATALYST
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<CommunityToolkit.Maui.Views.MediaElement, Handler.CustomMediaElementHandler>();
            });
#endif

            return builder.Build();
        }
    }
}