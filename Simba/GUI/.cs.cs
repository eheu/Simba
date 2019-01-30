using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simba
{
    public partial class ControlsForm : Form
    {
        private Board Savannah = SavannahForm.Savannah;
        private MethodInvoker RefreshLabelsInvoker { get => delegate { RefreshLabels(); }; }
        private static bool started;
        private Label lionCounter;
        private Label gnuCounter;

        public ControlsForm(SavannahForm savannahForm)
        {
            SavannahForm = savannahForm;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.LimeGreen;
            TransparencyKey = BackColor;
            InitializeComponent();
            AddControls();
            FormBorderStyle = FormBorderStyle.None;
            //WindowState = FormWindowState.Maximized;
            Owner = SavannahForm;
            TopLevel = true;
            Location = new Point((SavannahForm.Bounds.Width - SavannahForm.AmountHorizontalFields * SavannahForm.ImagesWidth) / 2, SavannahForm.AmountVerticalFields * 50);
            AutoSize = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.LimeGreen, e.ClipRectangle);
        }

        public static SavannahForm SavannahForm { get; set; }

        public Button StartPauseButton { get; set; }

        public int MaxControlWidth { get; private set; }

        private void AddControls()
        {
            int labelWidth = 100;
            int labelHeight = 50;
            lionCounter = new Label
            {
                Size = new Size(labelWidth, labelHeight),
                //Location = new Point((Screen.FromControl(this).Bounds.Width - AmountHorizontalFields * ImagesWidth) / 2, AmountVerticalFields * 50),
                Text = " X " + Savannah.Lions.Count.ToString(),
                Image = Properties.Resources.tMaleAdultLion50x50,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.LimeGreen,
                Font = new Font("Comic Sans MS", 14.25F),
            };
            Controls.Add(lionCounter);

            gnuCounter = new Label()
            {
                Size = new Size(labelWidth, labelHeight),
                //Location = new Point(((Screen.FromControl(this).Bounds.Width) - AmountHorizontalFields * ImagesWidth) / 2, AmountVerticalFields * 50 + lionCounter.Height),
                Location = new Point(0, lionCounter.Height),
                Text = " X " + Savannah.Gnus.Count.ToString(),
                Image = Properties.Resources.tGnu,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.LimeGreen,
                Font = new Font("Comic Sans MS", 14.25F)
            };
            Controls.Add(gnuCounter);

            int buttonWidth = 200;
            int buttonHeight = 100;
            StartPauseButton = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LimeGreen,
                Image = Properties.Resources.startButton200x100,
                //Location = new Point((Screen.FromControl(this).Bounds.Width) / 2 - buttonWidth / 2, AmountVerticalFields * 50),
                Location = new Point(gnuCounter.Width, 0),
            };
            StartPauseButton.FlatAppearance.BorderSize = 0;
            StartPauseButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            StartPauseButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            StartPauseButton.FlatAppearance.CheckedBackColor = Color.Transparent;
            Controls.Add(StartPauseButton);
            StartPauseButton.Click += new EventHandler(StartPause_Click);
        }
        public void StartPause_Click(object sender, System.EventArgs e)
        {
            started = !started;

            if (started)
            {
                StartPauseButton.Image = Properties.Resources.pauseButton200x100;

                Thread t = new Thread(
                    () =>
                    {
                        while (started)
                        {
                            SavannahForm.PlayRound();
                        }
                    }
                );
                t.Start();
            }
            else
                StartPauseButton.Image = Properties.Resources.startButton200x100;
        }

        public static void StartPause()
        {
            started = !started;
        }

        private void RefreshLabelsThreadSafe()
        {
            if (InvokeRequired)
                Invoke(RefreshLabelsInvoker);
            else
                Refresh();
        }


        private void RefreshLabels()
        {
            gnuCounter.Text = " X " + Savannah.Gnus.Count.ToString();
            lionCounter.Text = " X " + Savannah.Lions.Count.ToString();
        }
    }
}
