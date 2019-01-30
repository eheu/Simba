using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Simba
{
    public partial class SavannahForm
    {
        private MethodInvoker RefreshLabelsInvoker { get => delegate { RefreshLabels(); }; }
        private bool started;

        public Timer Timer { get; private set; }
        private Label lionCounter;
        private Label gnuCounter;
        public Button StartPauseButton { get; set; }

        public int MaxControlWidth { get; private set; }

        private void AddControls()
        {
            int labelWidth = 100;
            int labelHeight = 50;
            lionCounter = new Label
            {
                Size = new Size(labelWidth, labelHeight),
                Location = new Point((Screen.FromControl(this).Bounds.Width - AmountHorizontalFields * ImagesWidth) / 2, AmountVerticalFields * 50),
                Text = " X " + Savannah.Lions.Count.ToString(),
                Image = Properties.Resources.tMaleAdultLion50x50,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Font = new Font("Comic Sans MS", 14.25F)
            };
            Controls.Add(lionCounter);

            gnuCounter = new Label()
            {
                Size = new Size(labelWidth, labelHeight),
                Location = new Point(((Screen.FromControl(this).Bounds.Width) - AmountHorizontalFields * ImagesWidth) / 2, AmountVerticalFields * 50 + lionCounter.Height),
                Text = " X " + Savannah.Gnus.Count.ToString(),
                Image = Properties.Resources.tGnu,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Font = new Font("Comic Sans MS", 14.25F)
            };
            Controls.Add(gnuCounter);

            int buttonWidth = 200;
            int buttonHeight = 100;
            StartPauseButton = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Image = Properties.Resources.startButton200x100,
                Location = new Point((Screen.FromControl(this).Bounds.Width) / 2 - buttonWidth / 2, AmountVerticalFields * 50),
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
                            PlayRound();
                        }
                    }
                );
                t.Start();
            }
            else
                StartPauseButton.Image = Properties.Resources.startButton200x100;
        }

        public void StartPause_Click()
        {
            started = !started;

            if (started)
            {
                //SavannahForm.StartPauseButton.Image = Properties.Resources.pauseButton200x100;

                Thread t = new Thread(
                    () =>
                    {
                        while (started)
                        {
                            PlayRound();
                        }
                    }
                );
                t.Start();
            }
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
