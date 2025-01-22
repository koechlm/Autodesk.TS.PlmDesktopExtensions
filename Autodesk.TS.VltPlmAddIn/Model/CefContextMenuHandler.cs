using CefSharp;
using CefSharp.WinForms;

public class CustomContextMenuHandler : IContextMenuHandler
{
    private const int InspectElementCommandId = 26501; // Custom command ID for "Inspect"

    public void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
    {
        // Clear the existing menu
        model.Clear();

        // Add default context menu items
        model.AddItem(CefMenuCommand.Reload, "Refresh");
        model.AddSeparator();
        model.AddItem((CefMenuCommand)InspectElementCommandId, "Inspect");
    }

    public bool OnContextMenuCommand(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
    {
        // Handle the context menu commands
        switch (commandId)
        {
            case CefMenuCommand.Reload:
                browser.Reload();
                return true;
            case (CefMenuCommand)InspectElementCommandId:
                browser.GetHost().ShowDevTools();
                return true;
            default:
                return false;
        }
    }

    public void OnContextMenuDismissed(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame)
    {
        // No action needed
    }

    public bool RunContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
    {
        // Use the default context menu
        return false;
    }
}
