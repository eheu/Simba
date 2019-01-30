using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Simba
{
    partial class Database
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["Simba.Properties.Settings.connectionString"].ConnectionString;

        private void ResetRows()
        {
            string query = "DELETE FROM Animal " +
                           "DELETE FROM Field " +
                           "DELETE FROM Board " +
                           "DBCC CHECKIDENT('Animal', RESEED, 0) " +
                           "DBCC CHECKIDENT('Field', RESEED, 0) " +
                           "DBCC CHECKIDENT('Board', RESEED, 0)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void SaveBoardToDatabase(Board board)
        {
            ResetRows();
            CreateBoard(board);
            CreateFields(board.Fields);
            CreateAnimals(board.Lions.Concat(board.Gnus), board);
            UpdateFieldsAnimalRow(board.Fields);
        }

        public Board LoadBoardFromDatabase()
        {
            Board board = ReadLatestBoard();
            if (board != null)
            {
                board.Fields = ReadFields(board);
                Tuple<List<Animal>, List<Animal>> lionsAndGnus = ReadAnimals(board);
                board.Lions = lionsAndGnus.Item1;
                board.Gnus = lionsAndGnus.Item2;
                board.SetDirectionalFields();
            }
            return board;
        }      
    }
}
