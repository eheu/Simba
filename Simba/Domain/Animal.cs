using System;
using System.Collections.Generic;

namespace Simba
{
    using static DirectionType;

    public abstract class Animal
    {
        private readonly Util.Random randomNumberSupplier = Util.Random.Instance();

        private List<DirectionType> availableDirections = new List<DirectionType>()
        {
            None,
            North,
            East,
            South,
            West,
            NorthEast,
            SouthEast,
            SouthWest,
            NorthWest,
        };

        public int ID { get; set; }

        public double Weight { get; set; }
         
        public int Age { get; set; }

        public bool IsMale { get; set; }

        public bool IsChild { get; set; }

        public Field Field { get; set; }

        public Field PreviousField { get; set; }

        public void Move()
        {
            var direction = availableDirections.Count > 1 ? availableDirections[randomNumberSupplier.Next(1, availableDirections.Count)] : None;
            if (direction == None)
            {
                if (PreviousField != null && PreviousField.Animal == null)
                {
                    MoveToField(PreviousField);
                }
            }
            switch (direction)
            {
                case None:
                    break;
                case North:
                    if (Field.NorthField != null && Field.NorthField.Animal == null && Field.NorthField != PreviousField)
                        MoveToDirection(North);
                    else
                    {
                        availableDirections.Remove(North);
                        Move();
                    }
                    break;

                case East:
                    if (Field.EastField != null && Field.EastField.Animal == null && Field.EastField != PreviousField)
                        MoveToDirection(East);
                    else
                    {
                        availableDirections.Remove(East);
                        Move();
                    }
                    break;
                case South:
                    if (Field.SouthField != null && Field.SouthField.Animal == null &&  Field.SouthField != PreviousField)
                        MoveToDirection(South);
                     else
                    {
                        availableDirections.Remove(South);
                        Move();
                    }
                    break;
                case West:
                    if (Field.WestField != null && Field.WestField.Animal == null && Field.WestField != PreviousField)
                        MoveToDirection(West);
                    else
                    {
                        availableDirections.Remove(West);
                        Move();
                    }
                    break;
                case NorthEast:
                    if (Field.NorthEastField != null && Field.NorthEastField.Animal == null && Field.NorthEastField != PreviousField)
                        MoveToDirection(NorthEast);
                    else
                    {
                        availableDirections.Remove(NorthEast);
                        Move();
                    }
                    break;
                case SouthEast:
                    if (Field.SouthEastField != null && Field.SouthEastField.Animal == null && Field.SouthEastField != PreviousField)
                        MoveToDirection(SouthEast);
                    else
                    {
                        availableDirections.Remove(SouthEast);
                        Move();
                    }
                    break;
                case SouthWest:
                    if (Field.SouthWestField != null && Field.SouthWestField.Animal == null && Field.SouthWestField != PreviousField)
                        MoveToDirection(SouthWest);
                    else
                    {
                        availableDirections.Remove(SouthWest);
                        Move();
                    }
                    break;
                case NorthWest:
                    if (Field.NorthWestField != null && Field.NorthWestField.Animal == null)
                        MoveToDirection(NorthWest);
                    else
                    {
                        availableDirections.Remove(NorthWest);
                        Move();
                    }
                    break;
            }

            availableDirections = new List<DirectionType>()
            {
                None,
                North,
                East,
                South,
                West,
                NorthEast,
                SouthEast,
                SouthWest,
                NorthWest,
            };

            //Weight -= 10;

            Age++;
            Field.Age++;

            if (Age == 12)
            {
                if (IsChild)
                {
                    IsChild = false;
                    Weight += 100;
                }
            }
        }

        public void MoveToField(Field newField)
        {
            Field.Animal = null;
            PreviousField = Field;
            Field = newField;
            Field.Animal = this;
        }

        public void MoveToDirection(DirectionType direction)
        {
            Field newField = null;

            switch (direction)
            {
                case None:
                    break;
                case North:
                    newField = Field.NorthField;
                    break;
                case East:
                    newField = Field.EastField;
                    break;
                case South:
                    newField = Field.SouthField;
                    break;
                case West:
                    newField = Field.WestField;
                    break;
                case NorthEast:
                    newField = Field.NorthEastField;
                    break;
                case SouthEast:
                    newField = Field.SouthEastField;
                    break;
                case SouthWest:
                    newField = Field.SouthWestField;
                    break;
                case NorthWest:
                    newField = Field.NorthWestField;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            Field.Animal = null;
            PreviousField = Field;
            Field = newField;
            Field.Animal = this;
        }

        public Animal GetAnimalToGiveBirthTo()
        {
            for (int i = 0; i < Field.DirectionalFields.Count; i++)
            {
                Field field = Field.DirectionalFields[i];
                if (field != null && field.Animal != null && field.Animal.GetType() == GetType() && field.Animal.IsMale != IsMale && !field.Animal.IsChild)
                {
                    if (GetType() == typeof(Gnu))
                    {
                        Gnu gnu = new Gnu(true);
                        if (randomNumberSupplier.Next(0, 2) == 1)
                            gnu.IsMale = true;
                        return gnu;
                    }
                    else if (GetType() == typeof(Lion))
                    {
                        Lion lion = new Lion(true);
                        if (randomNumberSupplier.Next(0, 2) == 1)
                            lion.IsMale = true;
                        return lion;
                    }
                }
            }
            return null;
        }
    }
}