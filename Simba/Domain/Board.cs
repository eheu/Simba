using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static Simba.Util;
namespace Simba
{
    public partial class Board
    {
        private readonly Util.Random randomNumberSupplier = Util.Random.Instance();

        public Board()
        {
            InitializeBoard();
        }

        public Board(bool IsFromLoad)
        {
            // if board IsLoaded, SetDirectionalFields should be called when fully loaded
            if (!IsFromLoad)
                InitializeBoard();
        }

        public int ID { get; set; }

        public bool IsRaining { get; set; }


        public List<Animal> Lions { get; set; } = new List<Animal>();

        public List<Animal> Gnus { get; set; } = new List<Animal>();

        public List<Grass> Grass { get; set; } = new List<Grass>();

        public Field[,] Fields { get; set; }

        public List<Animal> MoveLions()
        {
            List<Animal> deadLions = new List<Animal>();
            for (int i = 0; i < Lions.Count; i++)
            {
                Lion lion = (Lion)Lions[i];
                lion.Move();
                if (lion.Weight < 0)
                {
                    deadLions.Add(lion);
                    lion.Field.Animal = null;
                    Lions.Remove(lion);
                }
            }

            return deadLions;
        }

        public List<Animal> MoveGnus()
        {
            List<Animal> deadGnus = new List<Animal>();
            for (int i = 0; i < Gnus.Count; i++)
            {
                Gnu gnu = (Gnu)Gnus[i];
                gnu.Move();
                if (gnu.Weight < 0)
                {
                    gnu.Field.Animal = null;
                    Gnus.Remove(gnu);
                    deadGnus.Add(gnu);
                }
            }
            return deadGnus;
        }

        public List<Grass> TryMakeGnusEat()
        {
            List<Grass> removedGrass = new List<Grass>();
            foreach (Gnu gnu in Gnus)
            {
                if (gnu.GetGrassToEat() != null)
                {
                    Grass grassToEat = gnu.GetGrassToEat();
                    Grass.Remove(gnu.Field.Grass);
                    removedGrass.Add(gnu.Field.Grass);
                    gnu.Field.Grass = null;
                }
            }
            return removedGrass;
        }

        public void MakeLionsEat()
        {
            foreach (Lion lion in Lions)
            {
                Gnu gnu = lion.GetGnuToEat();
                if (gnu != null)
                {
                    gnu.Field.Animal = null;
                    Gnus.Remove(gnu);
                }
            }
        }

        public List<Animal> TryMakeAnimalsBreed()
        {
            List<Animal> newAnimals = new List<Animal>();
            List<Animal> animals = Lions.Concat(Gnus).ToList();
            foreach (Animal animal in animals)
            {
                if (!animal.IsMale && !animal.IsChild)
                {
                    Animal newAnimal = animal.GetAnimalToGiveBirthTo();
                    if (newAnimal != null)
                    {
                        if (newAnimal.GetType() == typeof(Gnu))
                            Gnus.Add(newAnimal);
                        else if (newAnimal.GetType() == typeof(Lion))
                            Lions.Add(newAnimal);
                        for (int i = 0; i < animal.Field.DirectionalFields.Count; i++)
                        {
                            if (animal.Field.DirectionalFields[i] != null && animal.Field.DirectionalFields[i].Animal == null)
                            {
                                newAnimal.Field = animal.Field.DirectionalFields[i];
                                animal.Field.DirectionalFields[i].Animal = newAnimal;
                                i = animal.Field.DirectionalFields.Count;
                            }
                            else if (i == animal.Field.DirectionalFields.Count - 1)
                                for (int j = 0; j < 1; j++)
                                {
                                    int x = randomNumberSupplier.Next(0, Fields.GetLength(0));
                                    int y = randomNumberSupplier.Next(0, Fields.GetLength(1));
                                    if (Fields[x, y].Animal == null)
                                    {
                                        newAnimal.Field = Fields[x, y];
                                        Fields[x, y].Animal = newAnimal;
                                    }
                                    else if (Lions.Concat(Gnus).ToList().Count != Fields.Length)
                                        j--;
                                    else
                                        return new List<Animal>();
                                }
                        }
                        newAnimals.Add(newAnimal);
                    }
                }
            }
            return newAnimals;
        }

        public void TryMakeRain()
        {
            if (randomNumberSupplier.Next(0, 10) == 1)
                IsRaining = true;
        }

        public List<Grass> TryGrowGrass()
        {
            List<Grass> newGrass = new List<Grass>();

            for (int x = 0; x < Fields.GetLength(0); x++)
                for (int y = 0; y < Fields.GetLength(1); y++)
                {
                    Field field = Fields[x, y];
                    if (field.Grass == null)
                    {
                        if (randomNumberSupplier.Next(0, 10) == 1)
                        {
                            Grass grass = new Grass();
                            field.Grass = grass;
                            grass.Field = field;
                            Grass.Add(grass);
                            newGrass.Add(grass);
                        }
                    }
                }

            return newGrass;
        }


        public void SetDirectionalFields()
        {
            for (int x = 0; x < Fields.GetLength(0); x++)
                for (int y = 0; y < Fields.GetLength(1); y++)
                {
                    SetDirections(x, y);
                }
        }

        public void SetDirections(int x, int y)
        {
            if (y != 0)
                Fields[x, y].NorthField = Fields[x, y - 1];
            if (x != Fields.GetLength(0) - 1)
                Fields[x, y].EastField = Fields[x + 1, y];
            if (y != Fields.GetLength(1) - 1)
                Fields[x, y].SouthField = Fields[x, y + 1];
            if (x != 0)
                Fields[x, y].WestField = Fields[x - 1, y];
            if (y != 0 && x != Fields.GetLength(0) - 1)
                Fields[x, y].NorthEastField = Fields[x + 1, y - 1];
            if (y != Fields.GetLength(1) - 1 && x != Fields.GetLength(0) - 1)
                Fields[x, y].SouthEastField = Fields[x + 1, y + 1];
            if (y != Fields.GetLength(1) - 1 && x != 0)
                Fields[x, y].SouthWestField = Fields[x - 1, y + 1];
            if (y != 0 && x != 0)
                Fields[x, y].NorthWestField = Fields[x - 1, y - 1];
        }

        private void MakeLions(int amountLions)
        {
            for (int i = 0; i < amountLions; i++)
            {
                int x = randomNumberSupplier.Next(0, Fields.GetLength(0));
                int y = randomNumberSupplier.Next(0, Fields.GetLength(1));

                if (Fields[x, y].Animal == null)
                {
                    if (i == 0)
                    {
                        Lion lion = new Lion();
                        lion.IsMale = true;
                        Fields[x, y].Animal = lion;
                        lion.Field = Fields[x, y];
                        Lions.Add(lion);
                    }
                    else if (i == 1)
                    {
                        Lion lion = new Lion();
                        lion.IsMale = false;
                        Fields[x, y].Animal = lion;
                        lion.Field = Fields[x, y];
                        Lions.Add(lion);
                    }
                    else
                    {
                        Lion lion = new Lion();
                        if (randomNumberSupplier.Next(0, 2) == 1) lion.IsMale = true; //50% chance
                        Fields[x, y].Animal = lion;
                        lion.Field = Fields[x, y];
                        Lions.Add(lion);
                    }
                }
                else
                    i--;
            }
        }

        private void MakeGnus(int amountGnus)
        {
            for (int i = 0; i < amountGnus; i++)
            {
                int x = randomNumberSupplier.Next(0, Fields.GetLength(0));
                int y = randomNumberSupplier.Next(0, Fields.GetLength(1));

                if (Fields[x, y].Animal == null)
                {
                    if (i == 0)
                    {
                        Gnu gnu = new Gnu();
                        gnu.IsMale = true;
                        Fields[x, y].Animal = gnu;
                        gnu.Field = Fields[x, y];
                        Gnus.Add(gnu);
                    }
                    else if (i == 1)
                    {
                        Gnu gnu = new Gnu();
                        gnu.IsMale = false;
                        Fields[x, y].Animal = gnu;
                        gnu.Field = Fields[x, y];
                        Gnus.Add(gnu);
                    }
                    else
                    {
                        Gnu gnu = new Gnu();
                        if (randomNumberSupplier.Next(0, 2) == 1) gnu.IsMale = true;
                        Fields[x, y].Animal = gnu;
                        gnu.Field = Fields[x, y];
                        Gnus.Add(gnu);
                    }
                }
                else
                    i--;
            }
        }

        private void InitializeBoard()
        {
            MakeFields(AmountHorizontalFields, AmountVerticalFields);
            SetDirectionalFields();
            MakeLions(AmountLions);
            MakeGnus(AmountGnus);
        }

        private void InitializeLoadedBoard()
        {
            SetDirectionalFields();
        }

        private void MakeFields(int amountHorizontalFields, int amountVerticalFields)
        {
            Fields = new Field[amountHorizontalFields, amountVerticalFields];

            for (int x = 0; x < amountHorizontalFields; x++)
                for (int y = 0; y < amountVerticalFields; y++)
                {
                    Field field = new Field();
                    if (randomNumberSupplier.Next(0, 10) == 1) //10% chance for grass
                    {
                        Grass grass = new Grass();
                        Grass.Add(grass);
                        field.Grass = grass;
                        grass.Field = field;
                    }
                    Fields[x, y] = field;
                }
        }
    }
}
