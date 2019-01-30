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
        private MethodInvoker RefreshLabelsInvoker { get => delegate { RefreshLabels(); }; }
        private static bool started;
        private Bitmap ControlsBackgroundBuffer { get; set; }
        private Label lionCounter;
        private Label gnuCounter;
        private Database db = new Database();

        public SavannahForm SavannahFormToControl { get; set; }
        public Button StartPauseButton { get; set; }
        public Button SaveButton { get; set; }
        public Button LoadButton { get; set; }
        public Button ExitButton { get; set; }

        public ControlsForm(SavannahForm savannahForm)
        {
            Location = new Point(Screen.FromControl(this).Bounds.X, Screen.FromControl(this).Bounds.X);
            SavannahFormToControl = savannahForm;
            InitializeComponent();
            AddControls();
            FormBorderStyle = FormBorderStyle.None;
            Owner = SavannahFormToControl;
            TopLevel = true;
            AutoSize = true;
            BackgroundImage = Properties.Resources.Sand;
            BackgroundImageLayout = ImageLayout.Tile;
            Size = new Size(StartPauseButton.Width, lionCounter.Height + gnuCounter.Height + StartPauseButton.Height);
        }

        private void RefreshLabels()
        {
            gnuCounter.Text = " X " + SavannahFormToControl.Savannah.Gnus.Count.ToString();
            lionCounter.Text = " X " + SavannahFormToControl.Savannah.Lions.Count.ToString();
        }

        private void AddControls()
        {
            int labelWidth = 100;
            int labelHeight = 50;
            string text = SavannahFormToControl.Savannah.AmountLions.ToString();
            lionCounter = new Label
            {
                Size = new Size(labelWidth, labelHeight),
                Text = " X " + SavannahFormToControl.Savannah.Lions.Count.ToString(),
                Image = Properties.Resources.tMaleAdultLion50x50,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Font = new Font("Comic Sans MS", 14.25F),
            };
            Controls.Add(lionCounter);

            gnuCounter = new Label()
            {
                Size = new Size(labelWidth, labelHeight),
                Location = new Point(0, lionCounter.Height),
                Text = " X " + SavannahFormToControl.Savannah.Gnus.Count.ToString(),
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
                Location = new Point(0, lionCounter.Height + gnuCounter.Height),
            };
            StartPauseButton.FlatAppearance.BorderSize = 0;
            StartPauseButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            StartPauseButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            StartPauseButton.FlatAppearance.CheckedBackColor = Color.Transparent;
            Controls.Add(StartPauseButton);
            StartPauseButton.Click += new EventHandler(StartPause_Click);

            SaveButton = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Image = Properties.Resources.saveButton200x100,
                Location = new Point(0, lionCounter.Height + gnuCounter.Height + StartPauseButton.Height),
            };
            SaveButton.FlatAppearance.BorderSize = 0;
            SaveButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            SaveButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            SaveButton.FlatAppearance.CheckedBackColor = Color.Transparent;
            Controls.Add(SaveButton);
            SaveButton.Click += new EventHandler(Save_Click);

            LoadButton = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Image = Properties.Resources.loadButton200x100,
                Location = new Point(0, lionCounter.Height + gnuCounter.Height + +StartPauseButton.Height + SaveButton.Height),
            };
            LoadButton.FlatAppearance.BorderSize = 0;
            LoadButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            LoadButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            LoadButton.FlatAppearance.CheckedBackColor = Color.Transparent;
            Controls.Add(LoadButton);
            LoadButton.Click += new EventHandler(Load_Click);


            ExitButton = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Image = Properties.Resources.exitButton200x100,
                Location = new Point(0, lionCounter.Height + gnuCounter.Height + +StartPauseButton.Height + SaveButton.Height + LoadButton.Height),
            };
            ExitButton.FlatAppearance.BorderSize = 0;
            ExitButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
            ExitButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
            ExitButton.FlatAppearance.CheckedBackColor = Color.Transparent;
            Controls.Add(LoadButton);
            ExitButton.Click += new EventHandler(Exit_Click);
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
                            SavannahFormToControl.PlayRound();
                        }
                    }
                );
                t.Start();
            }
            else
                StartPauseButton.Image = Properties.Resources.startButton200x100;
        }

        public static void Pause()
        {
            started = false;
        }

        public void Save_Click(object sender, EventArgs e)
        {
            started = false;
            StartPauseButton.Image = Properties.Resources.startButton200x100;
            db.SaveBoardToDatabase(SavannahFormToControl.Savannah);
        }

        public void Load_Click(object sender, EventArgs e)
        {
            started = false;
            StartPauseButton.Image = Properties.Resources.startButton200x100;
            Board board = db.LoadBoardFromDatabase();
            if (board != null)
            {
                SavannahForm newSavannahForm = new SavannahForm(board, this);
                SavannahFormToControl.Hide();
                SavannahFormToControl = newSavannahForm;
                Owner = SavannahFormToControl;
                TopLevel = true;
                newSavannahForm.Show();
            }
        }

        public void Exit_Click(object sender, EventArgs e)
        {
            SavannahFormToControl.Close();
            Close();
        }

        public void RefreshLabelsThreadSafe()
        {
            if (InvokeRequired)
                Invoke(RefreshLabelsInvoker);
            else
                Refresh();
        }
    }
}
