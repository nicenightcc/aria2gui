namespace Aria2
{
    public class DragHandler : CefSharp.IDragHandler
    {

        public bool OnDragEnter(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IDragData dragData, CefSharp.DragOperationsMask mask)
        {
            return true;
        }

        public void OnDraggableRegionsChanged(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, System.Collections.Generic.IList<CefSharp.DraggableRegion> regions)
        {
        }
    }
}
