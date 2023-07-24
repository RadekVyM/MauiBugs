#if IOS || MACCATALYST

using CommunityToolkit.Maui.Core.Handlers;
using CommunityToolkit.Maui.Core.Views;

namespace iOSMediaElement.Handler;

public class CustomMediaElementHandler : MediaElementHandler
{
    public CustomMediaElementHandler() : base()
    {
    }

    public CustomMediaElementHandler(IPropertyMapper mapper, CommandMapper commandMapper)
        : base(mapper, commandMapper)
    {
    }


    protected override MauiMediaElement CreatePlatformView()
    {
        if (MauiContext is null)
        {
            throw new InvalidOperationException($"{nameof(MauiContext)} cannot be null");
        }

        mediaManager ??= new(MauiContext,
                                VirtualView,
                                Dispatcher.GetForCurrentThread() ?? throw new InvalidOperationException($"{nameof(IDispatcher)} cannot be null"));

        var (_, playerViewController) = mediaManager.CreatePlatformView();

        return new CustomMauiMediaElement(playerViewController, VirtualView);
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        PlatformView.Dispose();
    }
}

#endif