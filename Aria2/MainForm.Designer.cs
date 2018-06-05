namespace Aria2
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.opendir_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refresh_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.log_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webui_WebUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.options_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dldir_ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.setting_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exit_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.notify_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "Aria2";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.opendir_ToolStripMenuItem,
            this.refresh_ToolStripMenuItem,
            this.log_ToolStripMenuItem,
            this.webui_WebUIToolStripMenuItem,
            this.options_ToolStripMenuItem,
            this.exit_ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 176);
            // 
            // opendir_ToolStripMenuItem
            // 
            this.opendir_ToolStripMenuItem.Name = "opendir_ToolStripMenuItem";
            this.opendir_ToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.opendir_ToolStripMenuItem.Text = "打开下载目录";
            this.opendir_ToolStripMenuItem.Click += new System.EventHandler(this.opendir_ToolStripMenuItem_Click);
            // 
            // refresh_ToolStripMenuItem
            // 
            this.refresh_ToolStripMenuItem.Name = "refresh_ToolStripMenuItem";
            this.refresh_ToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.refresh_ToolStripMenuItem.Text = "刷新界面";
            this.refresh_ToolStripMenuItem.Click += new System.EventHandler(this.refresh_ToolStripMenuItem_Click);
            // 
            // log_ToolStripMenuItem
            // 
            this.log_ToolStripMenuItem.Name = "log_ToolStripMenuItem";
            this.log_ToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.log_ToolStripMenuItem.Text = "显示日志";
            this.log_ToolStripMenuItem.Click += new System.EventHandler(this.log_ToolStripMenuItem_Click);
            // 
            // webui_WebUIToolStripMenuItem
            // 
            this.webui_WebUIToolStripMenuItem.Name = "webui_WebUIToolStripMenuItem";
            this.webui_WebUIToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.webui_WebUIToolStripMenuItem.Text = "切换WebUI";
            // 
            // options_ToolStripMenuItem
            // 
            this.options_ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dldir_ToolStripMenuItem1,
            this.setting_ToolStripMenuItem,
            this.notify_ToolStripMenuItem});
            this.options_ToolStripMenuItem.Name = "options_ToolStripMenuItem";
            this.options_ToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.options_ToolStripMenuItem.Text = "设置";
            // 
            // dldir_ToolStripMenuItem1
            // 
            this.dldir_ToolStripMenuItem1.Name = "dldir_ToolStripMenuItem1";
            this.dldir_ToolStripMenuItem1.Size = new System.Drawing.Size(216, 26);
            this.dldir_ToolStripMenuItem1.Text = "设置下载目录";
            this.dldir_ToolStripMenuItem1.Click += new System.EventHandler(this.dldir_ToolStripMenuItem1_Click);
            // 
            // setting_ToolStripMenuItem
            // 
            this.setting_ToolStripMenuItem.Name = "setting_ToolStripMenuItem";
            this.setting_ToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.setting_ToolStripMenuItem.Text = "高级选项";
            this.setting_ToolStripMenuItem.Click += new System.EventHandler(this.setting_ToolStripMenuItem_Click);
            // 
            // exit_ToolStripMenuItem
            // 
            this.exit_ToolStripMenuItem.Name = "exit_ToolStripMenuItem";
            this.exit_ToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.exit_ToolStripMenuItem.Text = "退出";
            this.exit_ToolStripMenuItem.Click += new System.EventHandler(this.exit_ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notify_ToolStripMenuItem
            // 
            this.notify_ToolStripMenuItem.Name = "notify_ToolStripMenuItem";
            this.notify_ToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.notify_ToolStripMenuItem.Text = "通知";
            this.notify_ToolStripMenuItem.Click += new System.EventHandler(this.notify_ToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "MainForm";
            this.Text = "Aria2";
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exit_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webui_WebUIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem log_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem options_ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem opendir_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dldir_ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setting_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refresh_ToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem notify_ToolStripMenuItem;
    }
}

