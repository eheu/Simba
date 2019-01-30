using System.Collections.Generic;

namespace Simba
{
    public class Field
    {
        public int ID { get; set; }

        public Grass Grass { get; set; }

        public Animal Animal { get; set; }

        public int Age { get; set; }

        public List<Field> DirectionalFields => new List<Field> { NorthField, EastField, SouthField, WestField, NorthEastField, SouthEastField, SouthWestField, NorthWestField };

        public Field NorthField { get; set; }

        public Field EastField { get; set; }

        public Field SouthField { get; set; }

        public Field WestField { get; set; }

        public Field NorthEastField { get; set; }

        public Field SouthEastField { get; set; }

        public Field SouthWestField { get; set; }

        public Field NorthWestField { get; set; }

    }
}
