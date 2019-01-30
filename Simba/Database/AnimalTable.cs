using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Simba
{
    partial class Database
    {
        private void CreateAnimals(IEnumerable<Animal> animals, Board board)
        {
            string query = "INSERT INTO Animal " +
                           "VALUES (@Weight, @Age, @IsMale, @IsChild, @FieldID, @PreviousFieldID, @TypeID, @BoardID) " +
                           "SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("Weight", SqlDbType.Int);
                command.Parameters.Add("Age", SqlDbType.Int);
                command.Parameters.Add("IsMale", SqlDbType.Bit);
                command.Parameters.Add("IsChild", SqlDbType.Bit);
                command.Parameters.Add("FieldID", SqlDbType.Int);
                command.Parameters.Add("PreviousFieldID", SqlDbType.Int);
                command.Parameters.Add("TypeID", SqlDbType.Int);
                command.Parameters.Add("BoardID", SqlDbType.Int);
                connection.Open();
                foreach (Animal animal in animals)
                {
                    command.Parameters["Weight"].Value = animal.Weight;
                    command.Parameters["Age"].Value = animal.Age;
                    command.Parameters["IsMale"].Value = animal.IsMale;
                    command.Parameters["IsChild"].Value = animal.IsChild;
                    command.Parameters["FieldID"].Value = animal.Field.ID;
                    command.Parameters["PreviousFieldID"].Value = animal.PreviousField != null ? animal.PreviousField.ID : (object)DBNull.Value; //cast to object to allow conversion 
                    command.Parameters["TypeID"].Value = animal.GetType() == typeof(Lion) ? 1 : 2;
                    command.Parameters["BoardID"].Value = board.ID;
                    animal.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        private Tuple<List<Animal>, List<Animal>> ReadAnimals(Board board)
        {
            List<Animal> lions = new List<Animal>();
            List<Animal> gnus = new List<Animal>();

            string query;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                query = "SELECT * " +
                        "FROM Animal " +
                        "WHERE TypeID = 1 AND BoardID = " + board.ID;
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Animal lion = new Lion
                            {
                                ID = (int)dataReader["AnimalID"],
                                Weight = (int)dataReader["Weight"],
                                Age = (int)dataReader["Age"],
                                IsMale = (bool)dataReader["IsMale"],
                                IsChild = (bool)dataReader["IsChild"],
                            };
                            int? FieldID = (int?)dataReader["FieldID"];
                            if (!FieldID.Equals(null))
                            {
                                lion.Field = GetFieldFromID(FieldID, board.Fields);
                                lion.Field.Animal = lion;
                            }
                            if (dataReader["PreviousFieldID"] != DBNull.Value)
                            {
                                int? PreviousFieldID = (int?)dataReader["PreviousFieldID"];
                                lion.PreviousField = GetFieldFromID(PreviousFieldID, board.Fields);
                            }
                            lions.Add(lion);
                        }
                    }
                }
                query = "SELECT * " +
                        "FROM Animal " +
                        "WHERE TypeID = 2";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Animal gnu = new Gnu
                            {
                                ID = (int)dataReader["AnimalID"],
                                Weight = (int)dataReader["Weight"],
                                Age = (int)dataReader["Age"],
                                IsMale = (bool)dataReader["IsMale"],
                                IsChild = (bool)dataReader["IsChild"],
                            };
                            int? FieldID = (int?)dataReader["FieldID"];
                            if (!FieldID.Equals(null))
                            {
                                gnu.Field = GetFieldFromID(FieldID, board.Fields);
                                gnu.Field.Animal = gnu;
                            }
                            if (dataReader["PreviousFieldID"] != DBNull.Value)
                            {
                                int? PreviousFieldID = (int?)dataReader["PreviousFieldID"];
                                gnu.PreviousField = GetFieldFromID(PreviousFieldID, board.Fields);
                            }
                            gnus.Add(gnu);
                        }
                    }
                }
            }

            return Tuple.Create(lions, gnus);
        }

        private Field GetFieldFromID(int? ID, Field[,] fields)
        {
            if (ID != null)
            {
                foreach (Field field in fields)
                {
                    if (field.ID == ID)
                    {
                        return field;
                    }
                }
            }
            return null;
        }
    }
}
