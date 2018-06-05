using CefSharp.WinForms;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace Aria2
{
    public partial class MainForm : Form
    {
        private string webui = "";
        private bool closing = false;
        private bool hanging = false;
        private bool showing = false;
        private bool notify = true;
        private Aria2c aria2c = null;
        private InternalWebServer server = null;
        private ChromiumWebBrowser webBrowser1 = null;
        private Configuration config = null;
        private Dictionary<string, int> status = new Dictionary<string, int>();

        public MainForm(Aria2c aria2c, InternalWebServer server)
        {
            InitializeComponent();
            this.aria2c = aria2c;
            this.server = server;
        }
        public void Init()
        {
            InitNotify();

            this.webui = new DirectoryInfo(Directory.GetDirectories(server.WebRoot).FirstOrDefault()).Name;
            this.config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;

            if (settings.AllKeys.Contains("height") && int.Parse(settings["height"].Value) > 0)
                this.Height = int.Parse(settings["height"].Value);
            if (settings.AllKeys.Contains("width") && int.Parse(settings["width"].Value) > 0)
                this.Width = int.Parse(settings["width"].Value);

            if (settings.AllKeys.Contains("webui"))
                this.webui = settings["webui"].Value;
            if (settings.AllKeys.Contains("notify"))
                this.notify = bool.Parse(settings["notify"].Value);


            foreach (var ui in Directory.GetDirectories(server.WebRoot))
            {
                var item = new ToolStripMenuItem()
                {
                    Text = new DirectoryInfo(ui).Name
                };
                item.Click += (s, e) =>
                {
                    this.webui = ((ToolStripMenuItem)s).Text;
                    UseWebUI(this.webui);
                };
                this.webui_WebUIToolStripMenuItem.DropDownItems.Add(item);
            }

            this.notify_ToolStripMenuItem.Text = this.notify ? "关闭通知" : "开启通知";

            this.notifyIcon1.MouseClick += notifyIcon_Click;
            this.notifyIcon1.MouseDoubleClick += notifyIcon_Click;
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;


            this.webBrowser1 = new ChromiumWebBrowser("about:blank")
            {
                Dock = DockStyle.Fill,
                AllowDrop = false,
                MenuHandler = new MenuHandler(),
            };

            this.Controls.Add(webBrowser1);

            this.webBrowser1.TitleChanged += (s, e) => { this.Invoke(() => { this.Text = e.Title; }); };

            UseWebUI(this.webui);
        }

        private void InitNotify()
        {
            var ipc = new IPCHelper().Server("aria2channel", "aria2gui");
            ipc.DataReceived += (s, e) =>
            {
                Console.WriteLine(e);
                if (e == "show")
                {
                    this.ShowForm();
                }
                else
                {
                    var args = e.Split('\r');
                    if (args.Length < 4) return;
                    var file = new FileInfo(args[3]).FullName;
                    var id = args[1];
                    var msg = "";
                    switch (args[0])
                    {
                        case "__on_bt_download_complete":
                            msg = string.Format("BT任务下载完成：{0}", file);
                            break;

                        case "__on_download_complete":
                            try { File.Delete(file + ".aria2"); } catch { }
                            msg = string.Format("下载完成：{0}", file);
                            break;

                        case "__on_download_error":
                            msg = string.Format("下载出错：{0}", file);
                            break;

                        case "__on_download_pause":
                            break;

                        case "__on_download_start":
                            if (!status.Keys.Contains(id))
                            {
                                status.Add(id, 1);
                                msg = string.Format("开始下载：{0}", file);
                            }
                            break;

                        case "__on_download_stop":
                            break;
                    }
                    if (!this.notify || string.IsNullOrEmpty(msg)) return;
                    this.notifyIcon1.ShowBalloonTip(3000, "Aria2", msg, ToolTipIcon.Info);
                }
            };
        }

        private void ShowForm()
        {
            if (hanging)
                this.browserResume();
            if (!showing)
                this.Show();
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.Activate();
            this.Focus();
            this.TopMost = false;
        }

        private void UseWebUI(string webui)
        {
            var cookie = CefSharp.Cef.GetGlobalCookieManager();
            cookie.DeleteCookies();
            this.webBrowser1.Load(string.Format("http://localhost:{0}/{1}/", server.Port, webui));
            if (File.Exists(Path.Combine(server.WebRoot, webui, "favicon.ico")))
                this.notifyIcon1.Icon = new Icon(Path.Combine(server.WebRoot, webui, "favicon.ico"));
            else
            {
                this.notifyIcon1.Icon = this.Icon;
            }
            this.notifyIcon1.Visible = true;
        }

        private void notifyIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowForm();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closing && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.showing = false;
                this.Hide();
                browserSuspend();
            }
            else
            {
                browserResume();
                this.notifyIcon1.Visible = false;
                if (config != null)
                {
                    var settings = config.AppSettings.Settings;
                    if (settings.AllKeys.Contains("height"))
                        settings["height"].Value = this.Height.ToString();
                    else
                        settings.Add("height", this.Height.ToString());
                    if (settings.AllKeys.Contains("width"))
                        settings["width"].Value = this.Width.ToString();
                    else
                        settings.Add("width", this.Width.ToString());
                    if (settings.AllKeys.Contains("webui"))
                        settings["webui"].Value = this.webui;
                    else
                        settings.Add("webui", this.webui);
                    if (settings.AllKeys.Contains("notify"))
                        settings["notify"].Value = this.notify.ToString();
                    else
                        settings.Add("notify", this.notify.ToString());
                    config.Save();
                }
                if (webBrowser1 != null)
                    webBrowser1.GetBrowser().CloseBrowser(true);
                aria2c.Stop();
                server.Stop();
            }
        }

        private void exit_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.closing = true;
            this.Close();
        }

        private void log_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var log = new LogForm();
            log.FormClosing += (s, ea) => { aria2c.DataReceived -= log.Log; };
            aria2c.DataReceived += log.Log;
            log.Show();
        }

        private void setting_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var setting = new SettingForm(aria2c.Config);
            setting.Show();
            setting.FormClosing += (s, ea) =>
            {
                Aria2Config config = new Aria2Config();
                foreach (var p in config.GetType().GetProperties())
                {
                    p.SetValue(config, setting.TextBoxes.FirstOrDefault(en => en.Name == p.Name).Text);
                }
                aria2c.Stop();
                aria2c = new Aria2c(config);
                aria2c.Start();
            };
        }

        private void dldir_ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            this.aria2c.Config.__dir = dialog.SelectedPath;
            aria2c.Stop();
            aria2c = new Aria2c(this.aria2c.Config);
            aria2c.Start();
        }

        private void opendir_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", this.aria2c.Config.__dir);
        }

        private void refresh_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cookie = CefSharp.Cef.GetGlobalCookieManager();
            cookie.DeleteCookies();
            this.webBrowser1.GetBrowser().Reload(true);
        }
        private void notify_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notify = !this.notify;
            this.notify_ToolStripMenuItem.Text = this.notify ? "关闭通知" : "开启通知";
        }

        private void browserSuspend()
        {
            foreach (var p in Process.GetProcessesByName("CefSharp.BrowserSubprocess").Where(en => en.MainModule.FileName.StartsWith(Application.StartupPath)))
                ProcessMgr.SuspendProcess(p.Id);
            this.hanging = true;
        }
        private void browserResume()
        {
            if (hanging)
                foreach (var p in Process.GetProcessesByName("CefSharp.BrowserSubprocess").Where(en => en.MainModule.FileName.StartsWith(Application.StartupPath)))
                    ProcessMgr.ResumeProcess(p.Id);
            this.hanging = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            Init();
        }

    }
}
