namespace Simba
{
    public class Lion : Animal
    {
        public Lion()
        {
            Weight = 150;
        }

        public Lion(bool isChild)
        {
            IsChild = isChild;
            if (IsChild)
                Weight = 100;
        }

        public Gnu GetGnuToEat()
        {
            for (int i = 0; i < Field.DirectionalFields.Count; i++)
            {
                Field field = Field.DirectionalFields[i];
                if (field != null && field.Animal != null && field.Animal.GetType() == typeof(Gnu))
                {
                    Weight += 10;
                    return (Gnu)field.Animal;
                }
            }
            return null;
        }
    }
}