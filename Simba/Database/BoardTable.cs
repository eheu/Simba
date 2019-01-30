using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Simba
{
    partial class Database
    {
        private void CreateBoard(Board board)
        {
            string query = "INSERT INTO Board " +
                           "VALUES (@CountX, @CountY) " +
                           "SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("CountX", SqlDbType.Int).Value = board.Fields.GetLength(0);
                command.Parameters.Add("CountY", SqlDbType.Int).Value = board.Fields.GetLength(1);
                connection.Open();
                board.ID = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private Board ReadLatestBoard()
        {
            bool IsFromLoad = true;
            Board board = new Board(IsFromLoad);

            string query = "SELECT * " +
                           "FROM Board " +
                           "WHERE BoardID = " +
                           "(SELECT MAX(BoardID) FROM BOARD)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    try
                    {
                        dataReader.Read();
                        board.ID = (int)dataReader["BoardID"];
                        int countX = (int)dataReader["CountX"];
                        int countY = (int)dataReader["CountY"];
                        board.Fields = new Field[countX, countY];
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        System.Windows.Forms.MessageBox.Show("There was no saved game.");
                        return null;
                    }
                }
            }
            return board;
        }
    }
}
