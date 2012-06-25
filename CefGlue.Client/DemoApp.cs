namespace Xilium.CefGlue.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Xilium.CefGlue;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.IO;

    internal sealed class DemoApp : CefApp
    {
        internal static Logger logger = new Logger();

        private static bool _mono;

        static DemoApp()
        {
            _mono = Type.GetType("Mono.Runtime") != null;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            logger.Write("OnBeforeCommandLineProcessing: ProcessType={0} CommandLine=[{1}]", processType, commandLine);
        }

        protected override void UnwrapCommandLine(CefCommandLine commandLine)
        {
            base.UnwrapCommandLine(commandLine);
            logger.Write("UnwrapCommandLine: {0}", commandLine.ToString());
            logger.Write("Unwrapped: {0}", commandLine.ToString());
        }

        protected override void WrapCommandLine(CefCommandLine commandLine)
        {
            base.WrapCommandLine(commandLine);
            logger.Write("WrapCommandLine: {0}", commandLine.ToString());

            var programPath = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;

            if (commandLine.GetSwitchValue("type") == "gpu-process")
            {
                commandLine.SetProgram(Path.Combine(Path.GetDirectoryName(programPath), "cefclient.exe"));
            }
            else
            {
                if (_mono)
                {
                    var monoPath = commandLine.GetProgram();
                    commandLine.SetProgram(programPath);
                    commandLine.PrependWrapper("--llvm");
                    commandLine.SetProgram(monoPath);
                }
            }

            logger.Write("Wrapped: {0}", commandLine.ToString());
        }
    }
}
