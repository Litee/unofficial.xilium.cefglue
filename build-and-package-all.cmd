call build-net40.cmd
call build-net45.cmd

rem msbuild Xilium.CefGlue.sln /t:Build /p:Configuration=Release /p:TargetFrameworkVersion=v4.0 /p:TargetFrameworkProfile=Client /p:Platform=x86
rem msbuild Xilium.CefGlue.sln /t:Build /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:TargetFrameworkProfile= /p:Platform=x86

nuget pack Litee.Unofficial.CefGlue.nuspec -Version %APPVEYOR_BUILD_VERSION%
nuget pack Litee.Unofficial.CefGlue.WPF.nuspec -Version %APPVEYOR_BUILD_VERSION%