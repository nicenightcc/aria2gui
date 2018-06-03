using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Linq;
using System.Windows.Forms;

namespace Aria2
{
    public class Aria2c : IDisposable
    {
        private Process p = null;
        private string logfile = "";
        public Aria2Config Config { get; private set; }
        public Action<string> ReceiveData { get; set; }

        public Aria2c(Aria2Config config)
        {
            this.Config = config;
        }

        public Aria2c()
        {
            var cfgfile = Path.Combine(Application.StartupPath, "aria2c.json");
            if (File.Exists(cfgfile))
            {
                var reader = new StreamReader(cfgfile, Encoding.UTF8);
                try
                {
                    this.Config = JsonConvert.DeserializeObject<Aria2Config>(reader.ReadToEnd());
                }
                catch
                {
                    this.Config = new Aria2Config();
                }
                reader.Close();
            }
            else
            {
                this.Config = new Aria2Config();
                var writer = new StreamWriter(cfgfile, false, Encoding.UTF8);
                writer.Write(JsonConvert.SerializeObject(Config));
                writer.Close();
            }
        }

        public void Start()
        {
            this.logfile = Path.Combine(Application.StartupPath, Config.__log);
            try { File.Delete(logfile); } catch { }
            var sessionfile = Path.Combine(Application.StartupPath, Config.__input_file);
            if (!File.Exists(sessionfile))
                File.Create(sessionfile).Close();

            var args = Config.GetType().GetProperties()
                .Where(en => !string.IsNullOrEmpty((string)en.GetValue(Config)))
                .Select(en => string.Format("{0}={1}", en.Name.Replace('_', '-'), en.GetValue(Config)));

            p = new Process();
            p.StartInfo.Arguments = string.Join(" ", args);
            if (Environment.Is64BitOperatingSystem)
                p.StartInfo.FileName = "aria2c64.exe";
            else
                p.StartInfo.FileName = "aria2c32.exe";

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = Application.StartupPath;
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
                //Console.WriteLine(str);
                if (ReceiveData != null)
                {
                    try { ReceiveData(str); } catch { }
                }
            }
        }

        public void Stop()
        {
            try
            {
                var url = "http://localhost:6800/jsonrpc?method=Aria2.shutdown&id=0";
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                string retString = sr.ReadToEnd();
                Thread.Sleep(100);
                while (!p.HasExited)
                {
                    Thread.Sleep(100);
                }
            }
            catch { }
            if (p != null)
            {
                try { p.Kill(); } catch { }
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
            var cfgfile = Path.Combine(Application.StartupPath, "aria2c.json");
            var writer = new StreamWriter(cfgfile, false);
            try
            {
                writer.Write(JsonConvert.SerializeObject(this.Config));
            }
            catch { }
            writer.Close();
            Stop();
        }
    }
}
