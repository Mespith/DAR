using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARpracticum
{
    class TestMetSQLite
    {
        public TestMetSQLite()
        {
            Program.dbConnection.Open();
            
            String sql = "CREATE TABLE highscores (name VARCHAR(20), score INT)";
            SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);
            command.ExecuteNonQuery();
        }

        public void Insert()
        {
            String sql = "INSERT INTO highscores (name, score) VALUES ('Me', 9001)";
            SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);
            command.ExecuteNonQuery();
            sql = "INSERT INTO highscores (name, score) VALUES ('Myself', 6000)";
            command = new SQLiteCommand(sql, Program.dbConnection);
            command.ExecuteNonQuery();
            sql = "INSERT INTO highscores (name, score) VALUES ('And I', 9001)";
            command = new SQLiteCommand(sql, Program.dbConnection);
            command.ExecuteNonQuery();
        }

        public Dictionary<String, Int32> SelectAll()
        {
            Dictionary<String, Int32> highscores = new Dictionary<String, Int32>();

            String sql = "SELECT * FROM highscores ORDER BY score DESC";
            SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                highscores.Add(reader["name"].ToString(), (Int32)reader["score"]);
            }

            return highscores;
        }
    }
}
