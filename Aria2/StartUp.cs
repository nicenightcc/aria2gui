using CefSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Aria2
{
    public class StartUp
    {
        public void Start()
        {
            //For Windows 7 and above, best to include relevant app.manifest entries as well
            Cef.EnableHighDPISupport();

            var cefsettings = new CefSettings()
            {
                Locale = "zh-CN",
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.CurrentDirectory, "cache"),
                IgnoreCertificateErrors = true,
                LogSeverity = LogSeverity.Disable,
            };
            cefsettings.CefCommandLineArgs.Add("disable-gpu", "1");
            cefsettings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(cefsettings, performDependencyCheck: true, browserProcessHandler: null);


            var path = new FileInfo(Process.GetCurrentProcess().MainModule.FileName).DirectoryName;
            var resource = Path.Combine(path, "resource");
            var webuis = Directory.GetDirectories(resource);

            if (webuis.Length == 0)
            {
                MessageBox.Show("缺少WebUI资源");
                Application.Exit();
            }

            var portHelper = new PortHelper();
            var aria2c = new Aria2c();

            if (portHelper.GetUsedPorts().Contains(6800))
            {
                var choice = MessageBox.Show(
                    "检测到6800端口已被占用，请检查是否为aria2c程序。\r\n\r\n"
                    + "若为aria2c，是否关闭已开启的aria2c，并开启新的例程？\r\n\r\n"
                    + "选择“是”开启新的例程，选择“否”继续使用已开启的例程，选择“取消”退出程序"
                    , "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (choice == DialogResult.Yes)
                {
                    aria2c.Stop();
                    aria2c.Start();
                }
                else if (choice == DialogResult.No)
                {
                }
                else
                {
                    Environment.Exit(0);
                    return;
                }
            }
            else
            {
                aria2c.Start();
            }
            var port = portHelper.GetRandPort();
            var server = new InternalWebServer(resource, port);
            server.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(aria2c, server));
        }
    }
}
