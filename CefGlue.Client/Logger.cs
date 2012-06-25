namespace Xilium.CefGlue.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.IO;
    using System.Diagnostics;
    using System.Windows.Forms;
    using System.Threading;

    internal sealed class Logger
    {
        public void Write(string format, params object[] args)
        {
            Write(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public void Write(string text)
        {
            FileStream file = null;
            int retries = 20;
            do
            {
                try
                {
                    file = File.Open("client.log", FileMode.Append, FileAccess.Write, FileShare.Read);
                }
                catch (IOException ex)
                {
                    retries--;
                    if (retries <= 0)
                    {
                        retries = 20;
                        var result = MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.RetryCancel);
                        if (result == DialogResult.Cancel) throw;
                    }
                    Thread.Sleep(100);
                }
            }
            while (file == null);

            try
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                        "|{0}|{1}|{2}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                        ((uint)(Process.GetCurrentProcess().Id)).ToString().PadLeft(6),
                        text));
                }
            }
            finally
            {
                if (file != null) file.Close();
            }
        }
    }
}
