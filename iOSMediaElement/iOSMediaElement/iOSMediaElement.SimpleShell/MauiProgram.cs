﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SimpleToolkit.SimpleShell;

namespace iOSMediaElement.SimpleShell
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .UseSimpleShell()
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