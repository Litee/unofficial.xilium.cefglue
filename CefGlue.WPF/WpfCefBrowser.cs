﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xilium.CefGlue.Helpers.Log;

namespace Xilium.CefGlue.WPF
{
    public class WpfCefBrowser : ContentControl, IDisposable
    {
        private static readonly Key[] HandledKeys =
        {
            Key.Tab, Key.Home, Key.End, Key.Left, Key.Right, Key.Up, Key.Down
        };

        private bool _disposed;
        private bool _created;

        private Image _browserPageImage;
        private WriteableBitmap _browserPageBitmap;

        private int _browserWidth;
        private int _browserHeight;
        private bool _browserSizeChanged;

        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private WpfCefClient _cefClient;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupImageBitmap;

        private ToolTip _tooltip;
        private DispatcherTimer _tooltipTimer;

        Dispatcher _mainUiDispatcher;

        private readonly ILogger _logger;

        public WpfCefBrowser() : this(new NLogLogger("WpfCefBrowser"))
        {
        }

        public WpfCefBrowser(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            StartUrl = "about:blank";

            _popup = CreatePopup();

            _tooltip = new ToolTip();
            _tooltip.StaysOpen = true;
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Closed += TooltipOnClosed;

            _tooltipTimer = new DispatcherTimer();
            _tooltipTimer.Interval = TimeSpan.FromSeconds(0.5);

            KeyboardNavigation.SetAcceptsReturn(this, true);
            _mainUiDispatcher = Dispatcher.CurrentDispatcher;
        }

        #region Disposable

        ~WpfCefBrowser()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tooltipTimer != null)
                {
                    _tooltipTimer.Stop();
                }

                if (_browserPageImage != null)
                {
                    _browserPageImage.Source = null;
                    _browserPageImage = null;
                }

                if (_browserPageBitmap != null)
                {
                    _browserPageBitmap = null;
                }

                // 					if (this.browserPageD3dImage != null)
                // 						this.browserPageD3dImage = null;

                // TODO: What's the right way of disposing the browser instance?
                if (_browserHost != null)
                {
                    _browserHost.CloseBrowser();
                    _browserHost = null;
                }

                if (_browser != null)
                {
                    _browser.Dispose();
                    _browser = null;
                }
            }

            _disposed = true;
        }

        #endregion

        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadingStateChangeEventHandler LoadingStateChange;
        public event LoadErrorEventHandler LoadError;


        internal void OnLoadStart(CefFrame frame)
        { 
            if (this.LoadStart != null)
            {
                var e = new LoadStartEventArgs(frame);
                this.LoadStart(this, e);
            }
        }

        internal void OnLoadEnd(CefFrame frame, int httpStatusCode)
        {
            if(this.LoadEnd != null)
            {
                var e = new LoadEndEventArgs(frame, httpStatusCode);
                this.LoadEnd(this, e);
            }
        }
        internal void OnLoadingStateChange(bool isLoading, bool canGoBack, bool canGoForward)
        {
            if (this.LoadingStateChange != null)
            {
                var e = new LoadingStateChangeEventArgs(isLoading, canGoBack, canGoForward);
                this.LoadingStateChange(this, e);
            }
        }
        internal void OnLoadError(CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            if (this.LoadError != null)
            {
                var e = new LoadErrorEventArgs(frame, errorCode, errorText, failedUrl);
                this.LoadError(this, e);
            }
        }

        public string StartUrl { get; set; }
        public bool AllowsTransparency { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //this.browserPageD3dImage = new D3DImage();

            _browserPageImage = new Image()
            {
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.None
            };

            this.Content = _browserPageImage;
        }

        public void ExecuteJavaScript(string code, string url, int line)
        {
            if (_browser != null)
                this._browser.GetMainFrame().ExecuteJavaScript(code, url, line);
        }


        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);

            if (_browserPageImage != null)
            {
                var newWidth = (int)size.Width;
                var newHeight = (int)size.Height;

                _logger.Debug("BrowserResize. Old H{0}xW{1}; New H{2}xW{3}.", _browserHeight, _browserWidth, newHeight, newWidth);

                if (newWidth > 0 && newHeight > 0)
                {
                    if (!_created)
                    {
                        AttachEventHandlers(this); // TODO: ?

                        // Create the bitmap that holds the rendered website bitmap
                        _browserWidth = newWidth;
                        _browserHeight = newHeight;
                        _browserSizeChanged = true;

                        // Find the window that's hosting us
                        Window parentWnd = FindParentOfType<Window>(this);
                        if (parentWnd != null)
                        {
                            IntPtr hParentWnd = new WindowInteropHelper(parentWnd).Handle;

                            var windowInfo = CefWindowInfo.Create();
                            windowInfo.SetAsWindowless(hParentWnd, AllowsTransparency);

                            var settings = new CefBrowserSettings();
                            _cefClient = new WpfCefClient(this);

                            // This is the first time the window is being rendered, so create it.
                            CefBrowserHost.CreateBrowser(windowInfo, _cefClient, settings, !string.IsNullOrEmpty(StartUrl) ? StartUrl : "about:blank");

                            _created = true;
                        }
                    }
                    else
                    {
                        // Only update the bitmap if the size has changed
                        if (_browserPageBitmap == null || (_browserPageBitmap.Width != newWidth || _browserPageBitmap.Height != newHeight))
                        {
                            _browserWidth = newWidth;
                            _browserHeight = newHeight;
                            _browserSizeChanged = true;

                            // If the window has already been created, just resize it
                            if (_browserHost != null)
                            {
                                _logger.Trace("CefBrowserHost::WasResized to {0}x{1}.", newWidth, newHeight);
                                _browserHost.WasResized();
                            }
                        }
                    }
                }
            }

            return size;
        }

        private void AttachEventHandlers(WpfCefBrowser browser)
        {
            browser.GotKeyboardFocus += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        _browserHost.SendFocusEvent(true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in GotFocus()", ex);
                }
            };

            browser.LostKeyboardFocus += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        _browserHost.SendFocusEvent(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in LostFocus()", ex);
                }
            };

            browser.MouseLeave += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = 0,
                            Y = 0
                        };

                        mouseEvent.Modifiers = GetMouseModifiers();

                        _browserHost.SendMouseMoveEvent(mouseEvent, true);
                        //_logger.Debug("Browser_MouseLeave");
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in MouseLeave()", ex);
                }
            };

            browser.MouseMove += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };

                        mouseEvent.Modifiers = GetMouseModifiers();

                        _browserHost.SendMouseMoveEvent(mouseEvent, false);

                        //_logger.Debug(string.Format("Browser_MouseMove: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in MouseMove()", ex);
                }
            };

            browser.MouseDown += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Focus();

                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y,
                        };

                        mouseEvent.Modifiers = GetMouseModifiers();

                        if (arg.ChangedButton == MouseButton.Left)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, false, arg.ClickCount);
                        else if (arg.ChangedButton == MouseButton.Middle)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Middle, false, arg.ClickCount);
                        else if (arg.ChangedButton == MouseButton.Right)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Right, false, arg.ClickCount);

                        //_logger.Debug(string.Format("Browser_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in MouseDown()", ex);
                }
            };

            browser.MouseUp += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y,
                        };

                        mouseEvent.Modifiers = GetMouseModifiers();

                        if (arg.ChangedButton == MouseButton.Left)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, true, arg.ClickCount);
                        else if (arg.ChangedButton == MouseButton.Middle)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Middle, true, arg.ClickCount);
                        else if (arg.ChangedButton == MouseButton.Right)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Right, true, arg.ClickCount);

                        //_logger.Debug(string.Format("Browser_MouseUp: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in MouseUp()", ex);
                }
            };

            browser.MouseWheel += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y,
                        };

                        _browserHost.SendMouseWheelEvent(mouseEvent, 0, arg.Delta);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in MouseWheel()", ex);
                }
            };

            // TODO: require more intelligent processing
            browser.PreviewTextInput += (sender, arg) =>
            {
                if (_browserHost != null)
                {
                    _logger.Debug("TextInput: text {0}", arg.Text);

                    foreach (var c in arg.Text)
                    {
                        CefKeyEvent keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.Char,
                            WindowsKeyCode = (int)c,
                            // Character = c,
                        };

                        keyEvent.Modifiers = GetKeyboardModifiers();

                        _browserHost.SendKeyEvent(keyEvent);
                    }
                }

                arg.Handled = true;
            };

            // TODO: require more intelligent processing
            browser.PreviewKeyDown += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        //_logger.Debug(string.Format("KeyDown: system key {0}, key {1}", arg.SystemKey, arg.Key));
                        CefKeyEvent keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.RawKeyDown,
                            WindowsKeyCode = KeyInterop.VirtualKeyFromKey(arg.Key == Key.System ? arg.SystemKey : arg.Key),
                            NativeKeyCode = 0,
                            IsSystemKey = arg.Key == Key.System,
                        };

                        keyEvent.Modifiers = GetKeyboardModifiers();

                        _browserHost.SendKeyEvent(keyEvent);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in PreviewKeyDown()", ex);
                }

                arg.Handled = HandledKeys.Contains(arg.Key);
            };

            // TODO: require more intelligent processing
            browser.PreviewKeyUp += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        //_logger.Debug(string.Format("KeyUp: system key {0}, key {1}", arg.SystemKey, arg.Key));
                        CefKeyEvent keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.KeyUp,
                            WindowsKeyCode = KeyInterop.VirtualKeyFromKey(arg.Key == Key.System ? arg.SystemKey : arg.Key),
                            NativeKeyCode = 0,
                            IsSystemKey = arg.Key == Key.System,
                        };

                        keyEvent.Modifiers = GetKeyboardModifiers();

                        _browserHost.SendKeyEvent(keyEvent);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in PreviewKeyUp()", ex);
                }

                arg.Handled = true;
            };
            browser._popup.MouseMove += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };

                        mouseEvent.Modifiers = GetMouseModifiers();

                        _browserHost.SendMouseMoveEvent(mouseEvent, false);

                        //_logger.Debug(string.Format("Popup_MouseMove: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in Popup.MouseMove()", ex);
                }
            };
            browser._popup.MouseDown += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };

                        mouseEvent.Modifiers = GetMouseModifiers();

                        _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, true, 1);

                        //_logger.Debug(string.Format("Popup_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in Popup.MouseDown()", ex);
                }
            };
            browser._popup.MouseWheel += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);
                        int delta = arg.Delta;
                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };

                        mouseEvent.Modifiers = GetMouseModifiers();
                        _browserHost.SendMouseWheelEvent(mouseEvent, 0, delta);

                        //_logger.Debug(string.Format("MouseWheel: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in Popup.MouseWheel()", ex);
                }
            };
        }

        #region Handlers

        public void HandleAfterCreated(CefBrowser browser)
        {
            int width = 0, height = 0;

            bool hasAlreadyBeenInitialized = false;

            _mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                if (_browser != null)
                {
                    hasAlreadyBeenInitialized = true;
                }
                else
                {
                    _browser = browser;
                    _browserHost = _browser.GetHost();
                    // _browserHost.SetFocus(IsFocused);

                    width = (int)_browserWidth;
                    height = (int)_browserHeight;
                }
            }));

            // Make sure we don't initialize ourselves more than once. That seems to break things.
            if (hasAlreadyBeenInitialized)
                return;

            if (width > 0 && height > 0)
                _browserHost.WasResized();

            // 			mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            // 			{
            // 				if (!string.IsNullOrEmpty(this.initialUrl))
            // 				{
            // 					NavigateTo(this.initialUrl);
            // 					this.initialUrl = string.Empty;
            // 				}
            // 			}));
        }

        internal bool GetViewRect(ref CefRectangle rect)
        {
            bool rectProvided = false;
            CefRectangle browserRect = new CefRectangle();

            // TODO: simplify this
            //_mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            //{
                try
                {
                    // The simulated screen and view rectangle are the same. This is necessary
                    // for popup menus to be located and sized inside the view.
                    browserRect.X = browserRect.Y = 0;
                    browserRect.Width = (int)_browserWidth;
                    browserRect.Height = (int)_browserHeight;

                    rectProvided = true;
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in GetViewRect()", ex);
                    rectProvided = false;
                }
            //}));

            if (rectProvided)
            {
                rect = browserRect;
            }

            _logger.Debug("GetViewRect result provided:{0} Rect: X{1} Y{2} H{3} W{4}", rectProvided, browserRect.X, browserRect.Y, browserRect.Height, browserRect.Width);

            return rectProvided;
        }

        internal void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            Point ptScreen = new Point();

            _mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                try
                {
                    Point ptView = new Point(viewX, viewY);
                    ptScreen = PointToScreen(ptView);
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in GetScreenPoint()", ex);
                }
            }));

            screenX = (int)ptScreen.X;
            screenY = (int)ptScreen.Y;
        }

        internal void HandleViewPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            // When browser size changed - we just skip frame updating.
            // This is dirty precheck to do not do Invoke whenever is possible.
            if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight)) return;

            _mainUiDispatcher.Invoke(DispatcherPriority.Render, new Action(delegate
            {
                // Actual browser size changed check.
                if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight)) return;

                try
                {
                    if (_browserSizeChanged)
                    {
                        _browserPageBitmap = new WriteableBitmap((int)_browserWidth, (int)_browserHeight, 96, 96, AllowsTransparency ? PixelFormats.Bgra32 : PixelFormats.Bgr32, null);
                        _browserPageImage.Source = _browserPageBitmap;

                        _browserSizeChanged = false;
                    }

                    if (_browserPageBitmap != null)
                    {
                        DoRenderBrowser(_browserPageBitmap, width, height, dirtyRects, buffer);
                    }

                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in HandleViewPaint()", ex);
                }
            }));
        }

        internal void HandlePopupPaint(int width, int height, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            if (width == 0 || height == 0)
            {
                return;
            }

            _mainUiDispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(
                    () =>
                    {
                        int stride = width * 4;
                        int sourceBufferSize = stride * height;

                        _logger.Debug("RenderPopup() Bitmap H{0}xW{1}, Browser H{2}xW{3}", _popupImageBitmap.Height, _popupImageBitmap.Width, width, height);


                        foreach (CefRectangle dirtyRect in dirtyRects)
                        {
                            _logger.Debug(
                                string.Format(
                                    "Dirty rect [{0},{1},{2},{3}]",
                                    dirtyRect.X,
                                    dirtyRect.Y,
                                    dirtyRect.Width,
                                    dirtyRect.Height));

                            if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                            {
                                continue;
                            }

                            int adjustedWidth = dirtyRect.Width;

                            int adjustedHeight = dirtyRect.Height;

                            Int32Rect sourceRect = new Int32Rect(dirtyRect.X, dirtyRect.Y, adjustedWidth, adjustedHeight);

                            _popupImageBitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, dirtyRect.X, dirtyRect.Y);
                        }
                    }));
        }

        private void DoRenderBrowser(WriteableBitmap bitmap, int browserWidth, int browserHeight, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            int stride = browserWidth * 4;
            int sourceBufferSize = stride * browserHeight;

            _logger.Debug("DoRenderBrowser() Bitmap H{0}xW{1}, Browser H{2}xW{3}", bitmap.Height, bitmap.Width, browserHeight, browserWidth);

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            foreach (CefRectangle dirtyRect in dirtyRects)
            {
                _logger.Debug(string.Format("Dirty rect [{0},{1},{2},{3}]", dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height));

                if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                {
                    continue;
                }

                // If the window has been resized, make sure we never try to render too much
                int adjustedWidth = (int)dirtyRect.Width;
                //if (dirtyRect.X + dirtyRect.Width > (int) bitmap.Width)
                //{
                //    adjustedWidth = (int) bitmap.Width - (int) dirtyRect.X;
                //}

                int adjustedHeight = (int)dirtyRect.Height;
                //if (dirtyRect.Y + dirtyRect.Height > (int) bitmap.Height)
                //{
                //    adjustedHeight = (int) bitmap.Height - (int) dirtyRect.Y;
                //}

                // Update the dirty region
                Int32Rect sourceRect = new Int32Rect((int)dirtyRect.X, (int)dirtyRect.Y, adjustedWidth, adjustedHeight);
                bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, (int)dirtyRect.X, (int)dirtyRect.Y);

                // 			int adjustedWidth = browserWidth;
                // 			if (browserWidth > (int)bitmap.Width)
                // 				adjustedWidth = (int)bitmap.Width;
                // 
                // 			int adjustedHeight = browserHeight;
                // 			if (browserHeight > (int)bitmap.Height)
                // 				adjustedHeight = (int)bitmap.Height;
                // 
                // 			int sourceBufferSize = browserWidth * browserHeight * 4;
                // 			int stride = browserWidth * 4;
                // 
                // 			Int32Rect sourceRect = new Int32Rect(0, 0, adjustedWidth, adjustedHeight);
                // 			bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, 0, 0);
            }
        }

        internal void OnPopupShow(bool show)
        {
            if (_popup == null)
            {
                return;
            }

            _mainUiDispatcher.Invoke(new Action(() => _popup.IsOpen = show));
        }

        internal void OnPopupSize(CefRectangle rect)
        {
            _mainUiDispatcher.Invoke(
                new Action(
                    () =>
                    {
                        _popupImageBitmap = null;
                        _popupImageBitmap = new WriteableBitmap(
                            rect.Width,
                            rect.Height,
                            96,
                            96,
                            PixelFormats.Bgr32,
                            null);

                        _popupImage.Source = this._popupImageBitmap;

                        _popup.Width = rect.Width;
                        _popup.Height = rect.Height;
                        _popup.HorizontalOffset = rect.X;
                        _popup.VerticalOffset = rect.Y;
                    }));
        }

        internal bool OnTooltip(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _tooltipTimer.Stop();
                UpdateTooltip(null);
            }
            else
            {
                _tooltipTimer.Tick += (sender, args) => UpdateTooltip(text);
                _tooltipTimer.Start();
            }

            return true;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Finds a parent of the specific type
        /// </summary>
        private static T FindParentOfType<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject parentObj = VisualTreeHelper.GetParent(obj);
            if (parentObj == null)
                return null;

            // Try to type cast the parent to the desired type.
            // If the cast succeeds, we've found the desired parent.
            T parent = parentObj as T;
            if (parent != null)
                return parent;

            // If we get here, the current parent wasn't of the right type, so keep looking recursively
            return FindParentOfType<T>(parentObj);
        }

        private static CefEventFlags GetMouseModifiers()
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (Mouse.LeftButton == MouseButtonState.Pressed)
                modifiers |= CefEventFlags.LeftMouseButton;

            if (Mouse.MiddleButton == MouseButtonState.Pressed)
                modifiers |= CefEventFlags.MiddleMouseButton;

            if (Mouse.RightButton == MouseButtonState.Pressed)
                modifiers |= CefEventFlags.RightMouseButton;

            return modifiers;
        }

        private static CefEventFlags GetKeyboardModifiers()
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (Keyboard.Modifiers == ModifierKeys.Alt)
                modifiers |= CefEventFlags.AltDown;

            if (Keyboard.Modifiers == ModifierKeys.Control)
                modifiers |= CefEventFlags.ControlDown;

            if (Keyboard.Modifiers == ModifierKeys.Shift)
                modifiers |= CefEventFlags.ShiftDown;

            return modifiers;
        }

        private Popup CreatePopup()
        {
            var popup = new Popup
            {
                Child = this._popupImage = CreatePopupImage(),
                PlacementTarget = this,
                Placement = PlacementMode.Relative
            };

            return popup;
        }

        private Image CreatePopupImage()
        {
            var temp = new Image();

            RenderOptions.SetBitmapScalingMode(temp, BitmapScalingMode.NearestNeighbor);

            temp.Stretch = Stretch.None;
            temp.HorizontalAlignment = HorizontalAlignment.Left;
            temp.VerticalAlignment = VerticalAlignment.Top;
            temp.Source = _popupImageBitmap;

            return temp;
        }

        private void UpdateTooltip(string text)
        {
            _mainUiDispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(
                    () =>
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            _tooltip.IsOpen = false;
                        }
                        else
                        {
                            _tooltip.Placement = PlacementMode.Mouse;
                            _tooltip.Content = text;
                            _tooltip.IsOpen = true;
                            _tooltip.Visibility = Visibility.Visible;
                        }
                    }));

            _tooltipTimer.Stop();
        }

        private void TooltipOnClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Placement = PlacementMode.Absolute;
        }

        #endregion

        #region Methods

        public void NavigateTo(string url)
        {
            // Remove leading whitespace from the URL
            url = url.TrimStart();

            if (_browser != null)
                _browser.GetMainFrame().LoadUrl(url);
            else
                StartUrl = url;
        }

        public void LoadString(string content, string url)
        {
            // Remove leading whitespace from the URL
            url = url.TrimStart();

            if (_browser != null)
                _browser.GetMainFrame().LoadString(content, url);
        }

        public bool CanGoBack()
        {
            if (_browser != null)
                return _browser.CanGoBack;
            else
                return false;
        }

        public void GoBack()
        {
            if (_browser != null)
                _browser.GoBack();
        }

        public bool CanGoForward()
        {
            if (_browser != null)
                return _browser.CanGoForward;
            else
                return false;
        }

        public void GoForward()
        {
            if (_browser != null)
                _browser.GoForward();
        }

        public void Refresh()
        {
            if (_browser != null)
                _browser.Reload();
        }

        #endregion
    }
}
