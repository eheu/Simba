using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simba
{
    partial class Database
    {
        private void CreateFields(Field[,] fields)
        {
            string query = "INSERT INTO Field " +
                           "VALUES (NULL, @HasGrass) " +
                           "SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("HasGrass", SqlDbType.Bit);
                connection.Open();
                foreach (Field field in fields)
                {
                    command.Parameters["HasGrass"].Value = field.Grass == null ? 0 : 1;
                    field.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        private void UpdateFieldsAnimalRow(Field[,] fields)
        {
            string query = "UPDATE Field " +
                           "SET AnimalID = @AnimalID " +
                           "WHERE FieldID = @FieldID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("AnimalID", SqlDbType.Int);
                command.Parameters.Add("FieldID", SqlDbType.Int);
                connection.Open();
                foreach (Field field in fields)
                {
                    if (field.Animal != null)
                    {
                        command.Parameters["AnimalID"].Value = field.Animal.ID;
                        command.Parameters["FieldID"].Value = field.ID;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private Field[,] ReadFields(Board board)
        {
            int CountX = board.Fields.GetLength(0);
            int CountY = board.Fields.GetLength(1);
            Field[,] fields = new Field[CountX, CountY];

            string query = "SELECT * " +
                           "FROM Field";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    for (int x = 0; x < CountX; x++)
                    {
                        for (int y = 0; y < CountY; y++)
                        {
                            dataReader.Read();
                            int ID = (int)dataReader["FieldID"];
                            Field field = new Field();
                            field.ID = ID;
                            if ((int)dataReader["HasGrass"] == 1)
                            {
                                Grass grass = new Grass();
                                field.Grass = grass;
                                board.Grass.Add(grass);
                            }
                            fields[x, y] = field;
                        }
                    }
                }
            }

            return fields;
        }
    }
}
