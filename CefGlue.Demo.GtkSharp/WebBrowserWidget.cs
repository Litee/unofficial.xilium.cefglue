namespace CefGlue.Demo.GtkSharp
{
    using System;
    using Xilium.CefGlue;

    [System.ComponentModel.ToolboxItem(true)]
    public partial class WebBrowserWidget : Gtk.VBox   // TODO: use Gtk.Box but how auto-size child ?
    {
        private CefBrowser _browser;

        public WebBrowserWidget()
        {
        }

        protected override void OnRealized ()
        {
            base.OnRealized ();

            // ChildVisible = true;

            var windowInfo = new CefWindowInfo();
            windowInfo.Parent = Raw;

            var client = new WebClient(this);

            var settings = new CefBrowserSettings()
                {
                    // AuthorAndUserStylesDisabled = true,
                };

            CefBrowserHost.CreateBrowser(windowInfo, client, settings, "http://google.com");
        }

        internal void OnAfterBrowserCreated(CefBrowser browser)
        {
            _browser = browser;

            // this.Children[0].Visible = true;

            // var widget = new Gtk.Widget(_browser.GetHost().GetWindowHandle());
            // this.Children[0].SetSizeRequest(400, 400);
            // widget.SetSizeRequest(400, 400);

        }

        private class WebClient : CefClient
        {
            private WebBrowserWidget _core;
            private WebLifeSpanHandler _lifeSpanHandler;

            public WebClient(WebBrowserWidget core)
            {
                _core = core;
                _lifeSpanHandler = new WebLifeSpanHandler(_core);
            }

            protected override CefLifeSpanHandler GetLifeSpanHandler ()
            {
                return _lifeSpanHandler;
            }
        }

        private class WebLifeSpanHandler : CefLifeSpanHandler
        {
            private WebBrowserWidget _core;

            public WebLifeSpanHandler(WebBrowserWidget core)
            {
                _core = core;
            }

            protected override void OnAfterCreated (CefBrowser browser)
            {
                _core.OnAfterBrowserCreated(browser);
            }

        }
    }
}
