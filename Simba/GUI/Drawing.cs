using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Simba.Properties;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Simba
{
    public partial class SavannahForm
    {
        private MethodInvoker RefreshInvoker { get => delegate { Refresh(); }; }
        private static Dictionary<Animal, RectangleF> animalRectangles = new Dictionary<Animal, RectangleF>();
        private static Dictionary<Grass, RectangleF> grassRectangles = new Dictionary<Grass, RectangleF>();
        private static Dictionary<Field, RectangleF> backgroundRectangles = new Dictionary<Field, RectangleF>();
        private static Dictionary<Field, RectangleF> rain1Rectangles = new Dictionary<Field, RectangleF>();
        private static Dictionary<Field, RectangleF> rain2Rectangles = new Dictionary<Field, RectangleF>();
        private static Bitmap grassBuffer = null;
        private static Bitmap backgroundBuffer = null;
        private static Bitmap animalAndGrassBuffer = null;
        private static Bitmap rainBuffer1 = null;
        private static Bitmap rainBuffer2 = null;
        private int ticksWithRain;

        private void SavannahForm_Paint(object sender, PaintEventArgs e)
        {
            int x = (Bounds.Width - AmountHorizontalFields * ImagesWidth) / 2;
            DrawAllAnimalsAndGrassToAnimalBuffer();
            e.Graphics.DrawImage(animalAndGrassBuffer, x, 0);
            if (Savannah.IsRaining)
            {
                if (ticksWithRain % 2 == 0 && ticksWithRain <= ticksWithRainAllowed)
                    e.Graphics.DrawImage(rainBuffer1, x, 0);
                else if (ticksWithRain <= ticksWithRainAllowed)
                    e.Graphics.DrawImage(rainBuffer2, x, 0);
            }
        }

        private void UpdateGrassRectangles(List<Grass> affectedGrass)
        {
            if (affectedGrass.Count > 0)
            {
                for (int i = 0; i < affectedGrass.Count; i++)
                {
                    Grass grass = affectedGrass[i];

                    if (!grassRectangles.ContainsKey(grass))
                    {
                        RectangleF r = backgroundRectangles[grass.Field];
                        RectangleF grassRectangle = new RectangleF { X = r.X, Y = r.Y, Height = 50, Width = 50 };
                        grassRectangles.Add(grass, grassRectangle);
                        using (Graphics g = Graphics.FromImage(grassBuffer))
                        {
                            g.DrawImage(Resources.tGrass, grassRectangle);
                        }
                    }
                    else if (grassRectangles.ContainsKey(grass))
                    {
                        RectangleF grassRectangle = grassRectangles[grass];
                        using (Graphics g = Graphics.FromImage(grassBuffer))
                        {
                            g.DrawImage(Resources.Sand, grassRectangle);
                        }
                        grassRectangles.Remove(grass);
                    }
                }
                RefreshFormThreadSafe();
            }
        }

        private void UpdateAnimalRectangles(List<Animal> affectedAnimals)
        {
            if (affectedAnimals.Count > 0)
            {
                for (int i = 0; i < affectedAnimals.Count; i++)
                {
                    Animal animal = affectedAnimals[i];

                    if (!animalRectangles.ContainsKey(animal))
                    {
                        RectangleF r = backgroundRectangles[animal.Field];
                        RectangleF animalRectangle = new RectangleF { X = r.X, Y = r.Y, Height = 50, Width = 50 };
                        animalRectangles.Add(animal, animalRectangle);
                    }
                    else if (animalRectangles.ContainsKey(animal))
                    {
                        RectangleF animalRectangle = animalRectangles[animal];
                        using (Graphics g = Graphics.FromImage(grassBuffer))
                        {
                            g.DrawImage(Resources.Sand, animalRectangle);
                        }
                        animalRectangles.Remove(animal);
                    }
                }
                RefreshFormThreadSafe();
            }
        }

        private Task DrawAnimalMovements()
        {
            Dictionary<Animal, bool> animalDrawStatus = new Dictionary<Animal, bool>();

            for (int i = 0; animalDrawStatus.ContainsValue(false) || animalDrawStatus.Count == 0; i++)
                DrawAllRectanglesAtOnce(ref animalDrawStatus);

            animalDrawStatus.Clear();

            return Task.CompletedTask;
        }

        private void RefreshFormThreadSafe()
        {
            if (InvokeRequired)
                Invoke(RefreshInvoker);
            else
                try
                {
                    Refresh();
                }
                catch (InvalidOperationException)
                { 
                    Invoke(RefreshInvoker);
                }
        }

        private void DrawAllRectanglesAtOnce(ref Dictionary<Animal, bool> animalDrawStatus)
        {
            IEnumerable<Animal> animals = Savannah.Lions.Concat(Savannah.Gnus);

            foreach (Animal a in animals)
            {
                if (!animalDrawStatus.ContainsKey(a))
                {
                    animalDrawStatus.Add(a, false);

                }
                if (DrawMovementTowardsFieldToRectangle(a, DrawingMovementSpeed))
                {
                    animalDrawStatus[a] = true;
                }

            }

            if (Savannah.IsRaining)
                ticksWithRain++;

            RefreshFormThreadSafe();
        }

        private bool DrawMovementTowardsFieldToRectangle(Animal a, float speed)
        {
            float newX = backgroundRectangles[a.Field].X;
            float newY = backgroundRectangles[a.Field].Y;
            float oldX = animalRectangles[a].X;
            float oldY = animalRectangles[a].Y;

            float Δx = newX - oldX;
            float Δy = newY - oldY;
            float length = (float)Math.Sqrt(Δx * Δx + Δy * Δy);

            RectangleF r = animalRectangles[a];
            if (length > speed)
            {
                r.X += speed * Δx / length;
                r.Y += speed * Δy / length;
            }
            else
            {
                //already at destination
                r.X = newX;
                r.Y = newY;
                animalRectangles[a] = r;
                return true;
            }
            animalRectangles[a] = r;
            return false;
        }

        private void DrawAllAnimalsAndGrassToAnimalBuffer()
        {
            if (animalAndGrassBuffer == null)
            {
                animalAndGrassBuffer = new Bitmap(AmountHorizontalFields * ImagesWidth, AmountVerticalFields * ImagesHeight);
            }
            using (Graphics g = Graphics.FromImage(animalAndGrassBuffer))
            {
                g.Clear(default);

                g.DrawImage(grassBuffer, 0, 0);

                foreach (Animal a in Savannah.Lions.Concat(Savannah.Gnus))
                {
                    RectangleF r1 = new RectangleF();
                    try
                    {
                        r1 = animalRectangles[a];
                        if (a.PreviousField != null)
                        {
                            if (a.Field.Grass != null && a.PreviousField.Grass != null)
                            {
                                RectangleF r2 = grassRectangles[a.Field.Grass];
                                RectangleF r3 = grassRectangles[a.PreviousField.Grass];
                                if (r2.Y < r1.Y && r3.Y < r1.Y) //if underneath field and previousfield Grass
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                }
                                else if (r2.Y > r1.Y && r3.Y < r1.Y) //if above field grass and underneath previousfield grass
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                    g.DrawImage(Resources.tGrass, r2);
                                }
                                else if (r2.Y < r1.Y && r3.Y > r1.Y) //if underneath field grass and above previousfield grass
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                    g.DrawImage(Resources.tGrass, r3);
                                }
                                else if (r2.Y > r1.Y && r3.Y > r1.Y) //if above field grass and above previousfield grass
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                    g.DrawImage(Resources.tGrass, r2);
                                    g.DrawImage(Resources.tGrass, r3);
                                }
                                else
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                    g.DrawImage(Resources.tGrass, r2);
                                    g.DrawImage(Resources.tGrass, r3);
                                }
                            }
                            else if (a.Field.Grass == null && a.PreviousField.Grass != null)
                            {
                                RectangleF r3 = grassRectangles[a.PreviousField.Grass];
                                if (r3.Y < r1.Y)
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                }
                                else
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                    g.DrawImage(Resources.tGrass, r3);
                                }
                            }
                            else if (a.Field.Grass != null && a.PreviousField.Grass == null)
                            {
                                RectangleF r2 = grassRectangles[a.Field.Grass];
                                if (r2.Y < r1.Y)
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                }
                                else
                                {
                                    g.DrawImage(DecideAnimalImage(a), r1);
                                    g.DrawImage(Resources.tGrass, r2);
                                }
                            }
                            else if (a.Field.Grass == null && a.PreviousField.Grass == null)
                            {
                                g.DrawImage(DecideAnimalImage(a), r1);
                            }
                        }
                        else if (a.Field.Grass != null)
                        {
                            RectangleF r2 = grassRectangles[a.Field.Grass];
                            if (r2.Y < r1.Y || r2.Y == r1.Y)
                            {
                                g.DrawImage(DecideAnimalImage(a), r1);
                            }
                            else
                            {
                                g.DrawImage(DecideAnimalImage(a), r1);
                                g.DrawImage(Resources.tGrass, r2);
                            }

                        }
                        else
                        {
                            Image img = DecideAnimalImage(a);
                            g.DrawImage(img, r1);
                        }
                    }
                    catch (KeyNotFoundException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        MessageBox.Show("Game over!");
                        Application.Exit();
                    }
                    catch (NullReferenceException ex)
                    {
                        Debug.WriteLine(ex.Message); 
                        MessageBox.Show("Game over!");
                        Application.Exit();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        MessageBox.Show("Game over!");
                        Application.Exit();
                    }
                }
            }
        }

        private void DrawGrassToGrassBuffer()
        {
            if (grassBuffer == null)
            {
                grassBuffer = new Bitmap(AmountHorizontalFields * ImagesWidth, AmountVerticalFields * ImagesHeight);
            }
            using (Graphics g = Graphics.FromImage(grassBuffer))
            {
                foreach (Grass grass in Savannah.Grass)
                {
                    RectangleF r = grassRectangles[grass];
                    g.DrawImage(Resources.tGrass, r);
                }
            }
        }

        private void DrawBackgroundToBackgroundBuffer()
        {
            if (backgroundBuffer == null)
            {
                backgroundBuffer = new Bitmap(AmountHorizontalFields * ImagesWidth, AmountVerticalFields * ImagesHeight);
            }
            using (Graphics g = Graphics.FromImage(backgroundBuffer))
            {
                g.Clear(default(Color));

                foreach (Field f in Savannah.Fields)
                {
                    RectangleF r = backgroundRectangles[f];
                    g.DrawImage(Resources.Sand, r);
                }
            }
        }

        private void DrawRainToRainBuffer1()
        {
            if (rainBuffer1 == null)
            {
                rainBuffer1 = new Bitmap(AmountHorizontalFields * ImagesWidth, AmountVerticalFields * ImagesHeight);
            }
            using (Graphics g = Graphics.FromImage(rainBuffer1))
            {
                g.Clear(default(Color));

                foreach (Field f in Savannah.Fields)
                {
                    RectangleF r = rain1Rectangles[f];
                    g.DrawImage(Resources.rain150x50, r);
                }
            }
        }

        private void DrawRainToRainBuffer2()
        {
            if (rainBuffer2 == null)
            {
                rainBuffer2 = new Bitmap(AmountHorizontalFields * ImagesWidth, AmountVerticalFields * ImagesHeight);
            }
            using (Graphics g = Graphics.FromImage(rainBuffer2))
            {
                g.Clear(default(Color));

                foreach (Field f in Savannah.Fields)
                {
                    RectangleF r = rain2Rectangles[f];
                    g.DrawImage(Resources.rain250x50, r);
                }
            }
        }

        private void AddRectangles()
        {
            AddAnimalRectangles();
            AddGrassRectangles(Savannah);
            AddBackgroundRectangles();
            AddRainRectangles();
        }

        private static void AddGrassRectangles(Board Savannah)
        {
            for (int x = 0; x < Savannah.Fields.GetLength(0); x++)
                for (int y = 0; y < Savannah.Fields.GetLength(1); y++)
                    if (Savannah.Fields[x, y].Grass != null)
                    {
                        grassRectangles.Add(Savannah.Fields[x, y].Grass, new RectangleF { X = x * 50, Y = y * 50, Height = 50, Width = 50 });
                    }
        }

        private void AddAnimalRectangles()
        {
            for (int x = 0; x < Savannah.Fields.GetLength(0); x++)
                for (int y = 0; y < Savannah.Fields.GetLength(1); y++)
                    if (Savannah.Fields[x, y].Animal != null)
                        animalRectangles.Add(Savannah.Fields[x, y].Animal, new RectangleF { X = x * 50, Y = y * 50, Height = 50, Width = 50 });
        }

        private void AddBackgroundRectangles()
        {
            for (int x = 0; x < Savannah.Fields.GetLength(0); x++)
                for (int y = 0; y < Savannah.Fields.GetLength(1); y++)
                    backgroundRectangles.Add(Savannah.Fields[x, y], new RectangleF { X = x * 50, Y = y * 50, Width = 50, Height = 50 });
        }

        private void AddRainRectangles()
        {
            for (int x = 0; x < Savannah.Fields.GetLength(0); x++)
                for (int y = 0; y < Savannah.Fields.GetLength(1); y++)
                {
                    rain1Rectangles.Add(Savannah.Fields[x, y], new RectangleF { X = x * 50, Y = y * 50, Width = 50, Height = 50 });
                    rain2Rectangles.Add(Savannah.Fields[x, y], new RectangleF { X = x * 50, Y = y * 50, Width = 50, Height = 50 });
                }
        }

        private static Image DecideAnimalImage(Animal animal)
        {
            Image img = Resources.pauseButton200x100;
            if (animal != null)
            {
                if (animal.IsChild)
                {
                    if (animal.GetType() == typeof(Gnu))
                        img = animal.IsMale ? Resources.tMaleChildGnu : Resources.tFemaleChildGnu;
                    else if (animal.GetType() == typeof(Lion))
                        img = animal.IsMale ? Resources.tMaleChildLion50x50 : Resources.tFemaleChildLion50x50;
                }
                else
                {
                    if (animal.GetType() == typeof(Gnu))
                        img = animal.IsMale ? Resources.tGnu : Resources.tFemaleGnu;
                    else if (animal.GetType() == typeof(Lion))
                        img = animal.IsMale ? Resources.tMaleAdultLion50x50 : Resources.tFemaleAdultLion50x50;
                }
            }
            return img;
        }
    }
}
