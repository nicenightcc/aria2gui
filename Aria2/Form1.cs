using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Aria2
{
    public partial class Form1 : Form
    {
        private string webui = "AriaNg";
        private bool closing = false;
        private int port = 0;
        private string webRoot = Path.Combine(Environment.CurrentDirectory, "webui");
        Aria2c aria2c;
        InternalWebServer server;
        public Form1()
        {
            InitializeComponent();
            this.notifyIcon1.MouseClick += notifyIcon_Click;
            this.notifyIcon1.MouseDoubleClick += notifyIcon_Click;
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;

            aria2c = new Aria2c();
            aria2c.Start();
            this.Disposed += aria2c.Dispose;

            server = new InternalWebServer(webRoot);
            this.port = server.Start();
            this.Disposed += server.Dispose;

            this.webBrowser1.ScriptErrorsSuppressed = true; //禁用错误脚本提示 
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false; //禁用右键菜单 
            //this.webBrowser1.WebBrowserShortcutsEnabled = false; //禁用快捷键 
            this.webBrowser1.AllowWebBrowserDrop = false;//禁止拖拽
            this.webBrowser1.ScrollBarsEnabled = false;//禁止滚动条

            this.webBrowser1.DocumentTitleChanged += ChangeTitle;

            UseWebUI(this.webui);
        }

        private void ChangeTitle(object sender, EventArgs e)
        {
            this.Text = this.webBrowser1.DocumentTitle;
        }

        private void UseWebUI(string webui)
        {
            this.webBrowser1.Navigate(string.Format("http://localhost:{0}/{1}/", port, webui));
            this.notifyIcon1.Icon = new Icon(Path.Combine(webRoot, webui, "favicon.ico"));
            this.notifyIcon1.Visible = true;
        }

        private void notifyIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.Show();
            else if (e.Button == MouseButtons.Right)
            {
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
            }
        }

        private void exit_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.closing = true;
            this.Close();
        }

        private void webui_WebUIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.webui == "WebUI")
                this.webui = "AriaNg";
            else
                this.webui = "WebUI";
            UseWebUI(this.webui);
        }

        private void log_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var log = new Form2();
            log.Show();
            aria2c.ReceiveData = log.WriteLog;
        }
    }
}
