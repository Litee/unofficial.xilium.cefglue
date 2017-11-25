# CefGlue upgrade to modern CEF/Chromium versions

* CefGlue upgrades stopped for unknown reason at version 59, but many people must upgrade to modern versions of Chromium - e.g. to fix security issues. I have forked CefGlue and upgraded it to Chromium 62.
* I am lazy, so I am running checks for Windows only. If there will be a demand I can start other setups as well.
* Features added since v59 are implemented, but not tested:
    * CefExtensions
    * CefDisplayHandler.on\_auto\_resize
    * CefRequestContextHandler.on\_request\_context_initialized

# How to build

* Build the VS project
* Download CEF binaries. For Windows x64 it will be http://opensource.spotify.com/cefbuilds/cef_binary_3.3202.1686.gd665578_windows64.tar.bz2
* Unpack downloaded file and copy CEF binaries from `Release` and `Resources` folders into the folder with your app - e.g. into `C:\Users\John\Documents\unofficial.xilium.cefglue\CefGlue.Demo.WinForms\bin\Debug`
* Launch your app