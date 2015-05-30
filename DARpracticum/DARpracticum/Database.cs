using System;
using System.Data.SQLite;
using System.IO;

namespace DARpracticum
{
    class Database
    {
        private bool exists;

        public Database(bool _exists)
        {
            exists = _exists;
        }

        public bool CreateDatabase()
        {
            if (!exists)
            {
                Program.dbConnection.Open();

                Form1.WriteLine("Creating the autompg table.");

                String sql = "CREATE TABLE autompg (id integer NOT NULL, mpg real, cylinders integer, displacement real, horsepower real, weight real, acceleration real, model_year integer, origin integer, brand text, model text, type text, PRIMARY KEY (id));";
                SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    Form1.WriteLine(String.Format("Something went wrong: {0}", e));
                    return false;
                }

                Form1.WriteLine("Created autompg successfully.");
                Program.dbConnection.Close();

                return FillDB();
            }

            return true;
        }

        private bool FillDB()
        {
            Program.dbConnection.Open();

            // Lees het bestand.
            Form1.WriteLine("Trying to read the db_content.txt...");
            StreamReader reader;
            try
            {
                reader = new StreamReader("db_content.txt");
            }
            catch (Exception e)
            {
                Form1.WriteLine(String.Format("Something went wrong: {0}", e));
                return false;
            }

            String line = reader.ReadLine();
            Form1.WriteLine("Trying to excecute all the INSERT queries from file...");

            while(!String.IsNullOrEmpty(line))
            {
                SQLiteCommand command = new SQLiteCommand(line, Program.dbConnection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Form1.WriteLine(String.Format("Something went wrong: {0}", e));
                    return false;
                }

                line = reader.ReadLine();
            }
            Program.dbConnection.Close();
            Form1.WriteLine("Sucessfully inserted all the data.");

            return true;
        }
    }
}
