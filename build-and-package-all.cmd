mkdir target\net40
del /Q CefGlue\bin\Release\*.*
call build-net40.cmd
copy CefGlue\bin\Release\*.* target\net40

mkdir target\net45
del /Q CefGlue\bin\Release\*.*
call build-net45.cmd
copy CefGlue\bin\Release\*.* target\net45

nuget pack Litee.Unofficial.CefGlue.nuspec -Version %APPVEYOR_BUILD_VERSION%