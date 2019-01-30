using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simba
{
    public partial class SavannahForm : Form
    {
        private ControlsForm controls;

        public Board Savannah { get; set; } = null;

        public SavannahForm(Board board, ControlsForm controlsForm)
        {
            Savannah = board;

            InitializeComponent();
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            AddRectangles();
            grassBuffer = null;
            DrawGrassToGrassBuffer();
            //DrawBackgroundToBackgroundBuffer();
            DrawRainToRainBuffer1();
            DrawRainToRainBuffer2();

            BackgroundImageLayout = ImageLayout.Tile;
            BackgroundImage = Properties.Resources.Sand;

            controls = controlsForm;
        }

        public SavannahForm(Board board)
        {
            Savannah = board;

            InitializeComponent();
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            AddRectangles();
            DrawGrassToGrassBuffer();
            DrawBackgroundToBackgroundBuffer();
            DrawRainToRainBuffer1();
            DrawRainToRainBuffer2();

            BackgroundImageLayout = ImageLayout.Tile;
            BackgroundImage = Properties.Resources.Sand;

            controls = new ControlsForm(this);
            controls.Show();
        }

        public SavannahForm()
        {
            Savannah = new Board();

            InitializeComponent();
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            AddRectangles();
            DrawGrassToGrassBuffer();
            DrawBackgroundToBackgroundBuffer();
            DrawRainToRainBuffer1();
            DrawRainToRainBuffer2();

            BackgroundImageLayout = ImageLayout.Tile;
            BackgroundImage = Properties.Resources.Sand;

            controls = new ControlsForm(this);
            controls.Show();
        }

        public async void PlayRound()
        {
            Savannah.MakeLionsEat();
            controls.RefreshLabelsThreadSafe();
            RefreshFormThreadSafe();
            List<Animal> deadLions = Savannah.MoveLions();
            UpdateAnimalRectangles(deadLions);
            await DrawAnimalMovements();
            Savannah.MakeLionsEat();
            controls.RefreshLabelsThreadSafe();
            RefreshFormThreadSafe();

            List<Grass> eatenGrass1 = Savannah.TryMakeGnusEat();
            UpdateGrassRectangles(eatenGrass1);
            RefreshFormThreadSafe();
            List<Animal> deadGnus1 = Savannah.MoveGnus();
            UpdateAnimalRectangles(deadGnus1);
            await DrawAnimalMovements();
            List<Grass> eatenGrass2 = Savannah.TryMakeGnusEat();
            UpdateGrassRectangles(eatenGrass2);
            RefreshFormThreadSafe();
            Thread.Sleep(50);

            List<Grass> eatenGrass3 = Savannah.TryMakeGnusEat();
            UpdateGrassRectangles(eatenGrass3);
            RefreshFormThreadSafe();
            List<Animal> deadGnus2 = Savannah.MoveGnus();
            UpdateAnimalRectangles(deadGnus2);
            await DrawAnimalMovements();
            List<Grass> eatenGrass4 = Savannah.TryMakeGnusEat();
            UpdateGrassRectangles(eatenGrass4);
            RefreshFormThreadSafe();

            if (Savannah.IsRaining)
            {
                List<Grass> newGrass = Savannah.TryGrowGrass();
                UpdateGrassRectangles(newGrass);
                Savannah.IsRaining = false;
                ticksWithRain = 0;
            }

            List<Animal> newAnimals = Savannah.TryMakeAnimalsBreed();
            UpdateAnimalRectangles(newAnimals);
            //RefreshLabelsThreadSafe();
            RefreshFormThreadSafe();
            Savannah.TryMakeRain();

            if (Savannah.Gnus.Count == 0)
            {
                MessageBox.Show("Game over. Lions win!");
                ControlsForm.Pause();
            }
            else if (Savannah.Lions.Count == 0)
            {
                MessageBox.Show("Game over. Gnus win!");
                ControlsForm.Pause();
            }
            else if (Savannah.Gnus.Count == 0 && Savannah.Lions.Count == 0)
            {
                MessageBox.Show("Game over. Nobody wins!");
                ControlsForm.Pause();
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Environment.Exit(0); //stop all running threads
        }
    }
}