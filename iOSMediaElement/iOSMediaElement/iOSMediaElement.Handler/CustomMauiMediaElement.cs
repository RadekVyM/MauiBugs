#if IOS || MACCATALYST

using AVKit;
using CommunityToolkit.Maui.Core.Views;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Handlers;
using UIKit;

namespace iOSMediaElement.Handler;

public class CustomMauiMediaElement : MauiMediaElement
{
    AVPlayerViewController playerViewController = null;
    Element rootElement = null;


    public CustomMauiMediaElement(AVPlayerViewController playerViewController, Element virtualView) : base(playerViewController, null)
    {
        ArgumentNullException.ThrowIfNull(playerViewController.View);
        ArgumentNullException.ThrowIfNull(virtualView);

        this.playerViewController = playerViewController;

        // Zero out the original implementation
        RemoveFromParents();

        TrySetSubview(virtualView);
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (rootElement is not null)
        {
            rootElement.ParentChanging -= ElementParentChanging;
        }

        playerViewController = null;
        rootElement = null;
    }

    private void TrySetSubview(Element virtualView)
    {
        // If the search for a parent ViewController was not succesfull,
        // try to find and use the Page ViewController
        var parentViewController = FindParentController(virtualView) ?? FindPageController(virtualView);

        if (parentViewController is null)
        {
            // If no ViewController was found, try to wait for adding the view to a parent
            // For example, this is needed when the MediaElement is part of a CollectionView or CarouselView cell
            rootElement = FindRootElement(virtualView);
            rootElement.ParentChanging += ElementParentChanging;
        }
        else
        {
            if (rootElement is not null)
                rootElement.ParentChanging -= ElementParentChanging;
            rootElement = null;
        }

        SetSubviewWithParentController(parentViewController);
    }

    private void SetSubviewWithParentController(UIViewController parentViewController)
    {
        RemoveFromParents();

        playerViewController.View.Frame = Bounds;

#if IOS16_0_OR_GREATER || MACCATALYST16_1_OR_GREATER
        // On iOS 16+ and macOS 13+ the AVPlayerViewController has to be added to a parent ViewController, otherwise the transport controls won't be displayed.

        parentViewController ??= WindowStateManager.Default.GetCurrentUIViewController();

        if (parentViewController?.View is not null)
        {
            // Zero out the safe area insets of the AVPlayerViewController
            UIEdgeInsets insets = parentViewController.View.SafeAreaInsets;
            playerViewController.AdditionalSafeAreaInsets =
                new UIEdgeInsets(insets.Top * -1, insets.Left, insets.Bottom * -1, insets.Right);
            // Add the View from the AVPlayerViewController to the parent ViewController
            parentViewController.AddChildViewController(playerViewController);
            parentViewController.View.AddSubview(playerViewController.View);

            playerViewController.DidMoveToParentViewController(parentViewController);
        }
#endif

        AddSubview(playerViewController.View);
    }

    private void RemoveFromParents()
    {
        foreach (var subview in Subviews)
        {
            subview.RemoveFromSuperview();
        }

        playerViewController.RemoveFromParentViewController();
        playerViewController.View.RemoveFromSuperview();
    }

    private UIViewController FindParentController(Element element)
    {
        if (element is null)
        {
            return null;
        }

        // Does this element have a ViewController?
        if (element.Handler?.PlatformView is UIResponder responder &&
            responder.NextResponder is UIViewController viewController)
        {
            // Is it the right ViewController?
            UIViewController controller = GetCorrectController(viewController);

            if (controller is not null)
            {
                return controller;
            }
        }

        // Try to find the controller in the parent element
        if (element.Parent is not null)
        {
            return FindParentController(element.Parent);
        }

        return null;
    }

    private static UIViewController GetCorrectController(UIViewController controller)
    {
        return controller switch
        {
            NavigationRenderer => null, // If a NavigationPage is used, the Page ViewController needs to be used instead
            ShellFlyoutRenderer => controller, // Shell support
            UICollectionViewController => controller, // CollectionView and CarouselView support
            UINavigationController navigationController => navigationController.VisibleViewController, // SimpleShell support
            _ => null
        };
    }

    private static UIViewController FindPageController(Element element)
    {
        if (element is null)
        {
            return null;
        }

        if (element.Handler is PageHandler pageHandler)
        {
            return pageHandler.ViewController;
        }

        return FindPageController(element.Parent);
    }

    private static Element FindRootElement(Element element)
    {
        while (element.Parent is not null)
        {
            element = element.Parent;
        }

        return element;
    }

    private void ElementParentChanging(object sender, ParentChangingEventArgs e)
    {
        TrySetSubview(e.NewParent);
    }
}

#endif