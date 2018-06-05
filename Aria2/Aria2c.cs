using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Aria2
{
    public class Aria2c : IDisposable
    {
        private Process p = null;
        private string logfile = "";
        private string path = new FileInfo(Process.GetCurrentProcess().MainModule.FileName).DirectoryName;
        public Aria2Config Config { get; private set; }
        //public Action<string> ReceiveData { get; set; }
        public event EventHandler<string> DataReceived;

        public Aria2c(Aria2Config config)
        {
            this.Config = config;
        }

        public Aria2c()
        {
            ReadConfig();
        }

        private void ReadConfig()
        {
            var cfgfile = Path.Combine(path, "aria2c.json");
            if (File.Exists(cfgfile))
            {
                using (var reader = new StreamReader(cfgfile, Encoding.UTF8))
                {
                    this.Config = JsonConvert.DeserializeObject<Aria2Config>(reader.ReadToEnd());
                    reader.Close();
                }
            }
            else
            {
                this.Config = new Aria2Config();
            }

        }
        private void SaveConfig()
        {
            var cfgfile = Path.Combine(path, "aria2c.json");
            using (var writer = new StreamWriter(cfgfile, false))
            {
                writer.Write(JsonConvert.SerializeObject(this.Config));
                writer.Close();
            }
        }

        public void Start()
        {
            this.logfile = Path.Combine(path, Config.__log);
            try { File.Delete(logfile); } catch { }
            var sessionfile = Path.Combine(path, Config.__input_file);
            if (!File.Exists(sessionfile))
                File.Create(sessionfile).Close();

            this.Config.__dir = new DirectoryInfo(this.Config.__dir).FullName;

            var filename = Process.GetCurrentProcess().MainModule.FileName;

            var args = new List<string>();

            foreach (var p in Config.GetType().GetProperties())
            {
                var v = p.GetValue(Config)?.ToString() ?? "";
                if (p.Name.StartsWith("__on_"))
                {
                    var bat = Path.Combine(Path.GetTempPath(), string.Format("aria2{0}.bat", p.Name));
                    using (var writer = new StreamWriter(bat, false))
                    {
                        writer.Write(string.Format("{0} {1} %1 %2 %3", filename, p.Name));
                        writer.Close();
                    }
                    args.Add(string.Format("{0}={1}", p.Name.Substring(2).Replace('_', '-'), bat));
                }
                else if (!string.IsNullOrEmpty(v))
                {
                    args.Add(string.Format("{0}={1}", p.Name.Substring(2).Replace('_', '-'), v));
                }
            }

            var conf = Path.Combine(Path.GetTempPath(), "aria2c.conf");
            using (var writer = new StreamWriter(conf, false))
            {
                writer.Write(string.Join("\r\n", args));
                writer.Close();
            }

            p = new Process();
            p.StartInfo.Arguments = "--conf-path=\"" + conf + "\"";
            if (Environment.Is64BitOperatingSystem)
                p.StartInfo.FileName = "aria2c64.exe";
            else
                p.StartInfo.FileName = "aria2c32.exe";

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = path;
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
                if (DataReceived != null)
                {
                    try { DataReceived(this, str); } catch { }
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
            Stop();
        }
    }
}
