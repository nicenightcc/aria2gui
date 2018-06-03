using CefSharp.WinForms;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace Aria2
{
    public partial class MainForm : Form
    {
        private string webui = "";
        private bool closing = false;
        private Aria2c aria2c = null;
        private InternalWebServer server = null;
        private ChromiumWebBrowser webBrowser1 = null;
        private Configuration config = null;

        public MainForm(Aria2c aria2c, InternalWebServer server)
        {
            InitializeComponent();
            this.aria2c = aria2c;
            this.server = server;
            this.Disposed += aria2c.Dispose;
            this.Disposed += server.Dispose;
        }
        public void Init()
        {
            this.webui = new DirectoryInfo(Directory.GetDirectories(server.WebRoot).FirstOrDefault()).Name;
            this.config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            if (settings.AllKeys.Contains("height"))
                this.Height = int.Parse(settings["height"].Value);
            if (settings.AllKeys.Contains("width"))
                this.Width = int.Parse(settings["width"].Value);

            if (settings.AllKeys.Contains("webui"))
                this.webui = settings["webui"].Value;


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
                this.Show();
                this.Focus();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closing && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
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
                    config.Save();
                }
                if (webBrowser1 != null)
                    webBrowser1.GetBrowser().CloseBrowser(true);
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
            log.Show();
            aria2c.ReceiveData = log.WriteLog;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            Init();
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
            System.Diagnostics.Process.Start("explorer.exe", this.aria2c.Config.__dir);
        }

        private void refresh_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cookie = CefSharp.Cef.GetGlobalCookieManager();
            cookie.DeleteCookies();
            this.webBrowser1.GetBrowser().Reload(true);
        }
    }
}
