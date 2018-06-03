using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Aria2
{
    public partial class SettingForm : Form
    {
        private readonly Aria2Config config;
        public List<TextBox> TextBoxes = new List<TextBox>();
        public SettingForm(Aria2Config config)
        {
            this.config = config;
            explains = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resource1.explains);
            InitializeComponent();
        }

        private void Init()
        {
            this.panel1.VerticalScroll.Enabled = true;
            this.panel1.VerticalScroll.Visible = true;
            this.panel1.Scroll += (s, e) =>
            {
                this.panel1.VerticalScroll.Value = e.NewValue;
            };
            var top = 30;

            foreach (var p in this.config.GetType().GetProperties().Reverse())
            {
                var name = p.Name.Replace('_', '-');
                var explain = explains.Keys.Contains(p.Name) ? explains[p.Name] : "";
                var box = new GroupBox()
                {
                    Top = top,
                    Left = 30,
                    Height = 50,
                    Dock = DockStyle.Top
                };

                var name_label = new Label()
                {
                    Top = 20,
                    Left = 20,
                    Width = 200,
                    AutoEllipsis = true,
                    Text = name,
                };
                new ToolTip()
                {
                    IsBalloon = true,
                }.SetToolTip(name_label, name);
                box.Controls.Add(name_label);
                var textbox = new TextBox()
                {
                    Top = 15,
                    Left = 250,
                    Width = 300,
                    Text = p.GetValue(config).ToString(),
                    Name = p.Name
                };
                this.TextBoxes.Add(textbox);
                box.Controls.Add(textbox);

                var explain_label = new Label()
                {
                    Dock = DockStyle.Fill,
                    AutoEllipsis = true,
                    Text = "                                                                         " + explain
                };
                new ToolTip()
                {
                    IsBalloon = true,
                }.SetToolTip(explain_label, explain);
                box.Controls.Add(explain_label);

                this.panel1.Controls.Add(box);
            }
            this.panel1.Visible = true;
            this.panel1.Focus();
        }

        private void SettingForm_Resize(object sender, EventArgs ea)
        {
            this.panel1.VerticalScroll.Enabled = true;
            this.panel1.VerticalScroll.Visible = true;
            this.panel1.Scroll += (s, e) =>
            {
                this.panel1.VerticalScroll.Value = e.NewValue;
            };
        }

        private Dictionary<string, string> explains = new Dictionary<string, string>();

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            Init();
        }
    }
}
