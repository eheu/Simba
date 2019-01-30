namespace Simba
{
    public class Gnu : Animal
    {
        public Gnu()
        {
            Weight = 150;
        }

        public Gnu(bool isChild)
        {
            IsChild = isChild;
            if (IsChild)
                Weight = 100;
        }
        public Grass GetGrassToEat()
        {
            if (Field.Grass != null)
            {
                Grass grass = Field.Grass;
                Weight += 10;
                return grass;
            }
            return null;
        }
    }
}