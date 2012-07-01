namespace CefGlue.Demo.GtkSharp
{
    using System;
    using System.Reflection;
    using System.IO;
    using Gtk;
    using Xilium.CefGlue;

    class Program
    {
        public static int Main (string[] args)
        {
            Console.WriteLine("Running CefGlue.Demo.GtkSharp: {0}", string.Join(" ", args));

            try
            {
                CefRuntime.Load();
            }
            catch (DllNotFoundException ex)
            {
                Console.Error.WriteLine("Failed to load CEF runtime.");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
            catch (CefVersionMismatchException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 3;
            }

            var mainArgs = new CefMainArgs(args);
            var demoApp = new DemoApp();
            var exitCode = CefRuntime.ExecuteProcess(mainArgs, demoApp);
            if (exitCode != -1) return exitCode;

            var settings = new CefSettings
                {
                    BrowserSubprocessPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath), "cefclient"),
                    SingleProcess = false,
                    MultiThreadedMessageLoop = false,
                    LogSeverity = CefLogSeverity.Default,
                    LogFile = "cef.log",
                };

            CefRuntime.Initialize(mainArgs, settings, demoApp);

            Application.Init();

            using (MainWindow win = new MainWindow())
            {
                win.DeleteEvent += (o, a) => CefRuntime.QuitMessageLoop();
                win.Show();

                CefRuntime.RunMessageLoop();
            }

            Console.WriteLine("CefRuntime.Shutdown()");
            CefRuntime.Shutdown();
            return 0;
        }
    }
}