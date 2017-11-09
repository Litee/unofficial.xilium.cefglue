* This is a fork of CefGlue, which works with Chromium 62
* So far I have tested it only with Windows x64 and demo browser works fine
* Features added since v59 are implemented, but not tested:
** CefExtensions
** CefDisplayHandler.on_auto_resize
** CefRequestContextHandler.on_request_context_initialized

# How to build

* Download CEF binaries - http://opensource.spotify.com/cefbuilds/cef_binary_3.3202.1680.g700054b_windows64.tar.bz2
* Build the VS project
* Copy CEF binaries from `Release` and `Resources` folders into the folder with your app - e.g. into `C:\Users\John\Documents\unofficial.xilium.cefglue\CefGlue.Demo.WinForms\bin\Debug`
* Launch your app