# Litee.Unofficial.CefGlue

![AppVeyor Status](https://ci.appveyor.com/api/projects/status/ee8qvlr68hrjum64/branch/master?svg=true)

## Why fork?

CefGlue upgrades stopped for unknown reason at version 59, but many people need upgrades - e.g. to fix security issues. I need the same for my project, so forked CefGlue and upgraded it to Chromium 62 and 63.

## How stable is it?

* So far I have just added few new APIs (see the list below), so should be as stable as the original CefGlue
* I implemented new APIs, but did not test them much - not required for my project
* I am interested in Windows version only, so do not run any checks for Mono or Linux

## What has changed after the last *official* CefGlue v59?

* v60-62
  * [CefExtension](https://github.com/chromiumembedded/cef/blob/master/include/cef_extension.h)
  * [CefExtensionHandler](https://github.com/chromiumembedded/cef/blob/master/include/cef_extension_handler.h)
  * [CefDisplayHandler](https://github.com/chromiumembedded/cef/blob/master/include/cef_display_handler.h).on\_auto\_resize
  * [CefRequestContextHandler](https://github.com/chromiumembedded/cef/blob/master/include/cef_request_context_handler.h).on\_request\_context_initialized
* v63
  * [CefServer](https://github.com/chromiumembedded/cef/blob/master/include/cef_server.h)

## Where to get .NET binaries?

I am publishing CefGlue core binaries for .NET 4.0 and .NET 4.5 here: https://www.nuget.org/packages/Litee.Unofficial.CefGlue/

WPF wrapper is available here: https://www.nuget.org/packages/Litee.Unofficial.CefGlue.WPF/

## How to use published NuGet packages in WPF app?

* Add `Litee.Unofficial.CefGlue.WPF` package into your app
* Build your app
* Download *matching* CEF binaries from http://opensource.spotify.com/cefbuilds/index.html
* Unpack the archive. Copy all files from `Release` and `Resources` folders into your app folder.
* Launch your app. Tada!

## How to build CefGlue myself?

* Download CEF binaries from http://opensource.spotify.com/cefbuilds/index.html and unpack the archive
* Copy `include` folder from CEF into `CefGlue.Interop.Gen\include`. Manually remove `cef_thread.h` and `cef_waitable_event.h` - these two files should be excluded.
* Run `gen-cef3.cmd` within `CefGlue.Interop.Gen` folder. Note that you need Python 2.7 installed. In case you need to adjust path to Python binaries you can do it in `gen-cef3.cmd` file. This step will generate multiple C# files in `CefGlue` project.
* Build CefGlue binaries - e.g. by running `build-net40.cmd` in the root of the project
* If you just upgraded to a new version of CEF may see compilation errors - most typical fix is to add *new* generated files into `CefGlue` VS project

## License

This is a fork, so it follows Xilium.CefGlue license - namely: "This project is licensed under MIT License with portions of code licensed under New BSD License."