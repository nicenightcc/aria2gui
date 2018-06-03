using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            explains = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resource.explains);
            InitializeComponent();
            //this.panel1.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.panel1, true, null);
            //this.panel1.CellPaint += (s, e) =>
            //{
            //    Pen pen = new Pen(Color.Black);
            //    e.Graphics.DrawLine(pen, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.X + this.panel1.Width - 1, e.CellBounds.Y);
            //    //e.Graphics.DrawRectangle(pen, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.X + this.panel1.Width - 1, e.CellBounds.Y + this.panel1.Height - 1);
            //};
        }

        private void Init()
        {
            //this.panel1.VerticalScroll.Enabled = true;
            //this.panel1.VerticalScroll.Visible = true;
            //this.panel1.Scroll += (s, e) =>
            //{
            //    this.panel1.VerticalScroll.Value = e.NewValue;
            //};

            var props = this.config.GetType().GetProperties();

            this.panel1.ColumnCount = 3;
            this.panel1.RowCount = props.Length;

            this.panel1.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 180);
            this.panel1.ColumnStyles[1] = new ColumnStyle(SizeType.Absolute, 200);
            this.panel1.ColumnStyles[2] = new ColumnStyle(SizeType.AutoSize);

            for (var i = 0; i < props.Length; i++)
            {
                var p = props[i];

                var name = p.Name.Replace('_', '-');
                var explain = explains.Keys.Contains(p.Name) ? explains[p.Name] : "";

                var name_label = new Label()
                {
                    Padding = new Padding(2),
                    Height = 30,
                    Width = 150,
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoEllipsis = true,
                    Text = name,
                };
                new ToolTip()
                {
                    IsBalloon = true,
                }.SetToolTip(name_label, name);
                this.panel1.Controls.Add(name_label, 0, i);

                var textbox = new TextBox()
                {
                    Height = 30,
                    Width = 300,
                    Text = p.GetValue(config).ToString(),
                    Name = p.Name
                };
                this.TextBoxes.Add(textbox);
                this.panel1.Controls.Add(textbox, 1, i);

                var explain_label = new Label()
                {
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Fill,
                    AutoEllipsis = true,
                    Text = explain
                };
                new ToolTip()
                {
                    IsBalloon = true,
                }.SetToolTip(explain_label, explain);
                this.panel1.Controls.Add(explain_label, 2, i);
                this.panel1.RowStyles[i] = new RowStyle(SizeType.Absolute, 50);
            }
            //this.panel1.RowStyles[props.Length] = new RowStyle(SizeType.Absolute, 50);

            this.panel1.Visible = true;
            this.panel1.Focus();
        }

        private Dictionary<string, string> explains = new Dictionary<string, string>();

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            Init();
        }
    }
}
