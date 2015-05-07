using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARpracticum
{
    class Database
    {
        public Database()
        {
            Program.dbConnection.Open();

            String sql = "CREATE TABLE autompg (id integer NOT NULL, mpg real, cylinders integer, displacement real, horsepower real, weight real, acceleration real, model_year integer, origin integer, brand text, model text, type text, PRIMARY KEY (id));";
            SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);
            command.ExecuteNonQuery();

            Program.dbConnection.Close();
        }

        public void FillDB()
        {
            Program.dbConnection.Open();

            // Lees het bestand.
            StreamReader reader = new StreamReader("C:\\Users\\Jaimy\\Documents\\Universiteit Utrecht\\Data-analyse\\DAR\\DARpracticum\\DARpracticum\\db_content.txt");
            String line = reader.ReadLine();

            while(!String.IsNullOrEmpty(line))
            {
                SQLiteCommand command = new SQLiteCommand(line, Program.dbConnection);
                command.ExecuteNonQuery();

                line = reader.ReadLine();
            }
            Program.dbConnection.Close();
        }
    }
}
