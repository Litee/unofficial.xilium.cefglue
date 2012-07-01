namespace CefGlue.Demo.GtkSharp
{
    using System;
    using System.Reflection;
    using System.IO;
    using Xilium.CefGlue;

    internal sealed class DemoApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            Console.WriteLine("OnBeforeCommandLineProcessing: {0} {1}", processType, commandLine);

            var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
            path = Path.GetDirectoryName(path);
            commandLine.AppendSwitch("pack-file-path", Path.Combine(path, "cef.pak"));
            commandLine.AppendSwitch("locales-dir-path", Path.Combine(path, "locales"));
        }

        protected override void AppendExtraCommandLineSwitches (CefCommandLine commandLine)
        {
            Console.WriteLine("AppendExtraCommandLineSwitches: {0}", commandLine);

            var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;

            commandLine.SetProgram("");
            commandLine.PrependWrapper(path);
            //
            // commandLine.PrependWrapper(" "); // there is not working on unix environments, 'cause they passed as is
            commandLine.PrependWrapper("mono");

            Console.WriteLine("    -> {0}", commandLine);
        }
    }
}

