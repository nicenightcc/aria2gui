using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Aria2
{
    public class Aria2c : IDisposable
    {
        private Process p = null;
        private string logfile = "";
        public Action<string> ReceiveData = null;

        public Aria2c()
        {
            this.logfile = Path.Combine(Environment.CurrentDirectory, "Aria2.log");
            if (File.Exists(logfile))
                File.Delete(logfile);
        }

        public void Start()
        {
            p = new Process();
            var x64 = Environment.Is64BitOperatingSystem;
            p.StartInfo.Arguments = "--conf=Aria2.conf";
            if (x64)
                p.StartInfo.FileName = "aria2c64.exe";
            else
                p.StartInfo.FileName = "aria2c32.exe";

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceived);
            p.ErrorDataReceived += new DataReceivedEventHandler(OutputDataReceived);
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var str = e.Data?.Trim();
            if (!string.IsNullOrEmpty(str))
            {
                using (var sw = new StreamWriter(logfile, true))
                {
                    sw.WriteLine(str);
                }
                if (ReceiveData != null)
                {
                    try { ReceiveData(str); }
                    catch (Exception ex)
                    {
                    }
                }
                Console.WriteLine(str);
            }
        }

        public void Stop()
        {
            if (p != null)
            {
                try
                {
                    var url = "http://localhost:6800/jsonrpc?method=Aria2.shutdown&id=0";
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Method = "GET";
                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                    StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                    string retString = sr.ReadToEnd();
                    while (!p.HasExited)
                    {
                        Thread.Sleep(100);
                    }
                }
                catch { try { p.Kill(); } catch { } }

                p.Close();
                p.Dispose();
            }
            p = null;
        }
        public void Dispose(object sender, System.EventArgs args)
        {
            this.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
