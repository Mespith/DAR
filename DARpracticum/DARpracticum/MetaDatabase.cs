using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARpracticum
{
    class MetaDatabase
    {
        String[] tables = new String[]
        {
            "mpg",
            "displacement",
            "cylinders",
            "horsepower",
            "weight",
            "acceleration",
            "model_year",
            "origin",
            "brand",
            "model",
            "type"
        };

        long n;

        public MetaDatabase()
        {
            Program.dbConnection.Open();

            String query = "SELECT count(*) FROM autompg;";
            SQLiteCommand command = new SQLiteCommand(query, Program.dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            n = (long)reader[0];

            Program.dbConnection.Close();

            CreateTables();
            FillTables();
        }

        private void CreateTables()
        {
            Program.metaDbConnection.Open();
            String sql;

            foreach(String table in tables)
            {
                if (table == "brand" || table == "model" || table == "type")
                {
                    sql = String.Format("CREATE TABLE {0} (waarde text UNIQUE, idf real, qf real);", table);
                }
                else
                {
                    sql = String.Format("CREATE TABLE {0} (waarde real UNIQUE, idf real, qf real);", table);
                }
                
                SQLiteCommand command = new SQLiteCommand(sql, Program.metaDbConnection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch
                {
                    // Tables probably already exists.
                }
            }

            Program.metaDbConnection.Close();
        }

        public void FillTables()
        {
            Program.metaDbConnection.Open();
            Program.dbConnection.Open();

            List<String> stringValues = new List<String>();
            List<float> floatValues = new List<float>();

            foreach (String table in tables)
            {
                String sql = String.Format("SELECT {0} FROM autompg;", table);
                SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    String insert;

                    if (table == "brand" || table == "model" || table == "type")
                    {
                        insert = String.Format("INSERT INTO {0} VALUES ('{1}', 0, 0);", table, reader[table]);
                    }
                    else
                    {
                        insert = String.Format("INSERT INTO {0} VALUES ({1}, 0, 0);", table, reader[table]);
                    }

                    SQLiteCommand insertCommand = new SQLiteCommand(insert, Program.metaDbConnection);
                    try
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                    catch
                    {
                        // Value is not unique, but we dont care.
                    }
                }
            }

            Program.metaDbConnection.Close();
            Program.dbConnection.Close();
        }

        public void CalculateIDF()
        {
            Program.metaDbConnection.Open();
            List<String> values = new List<String>();

            foreach(String table in tables)
            {
                String sql = String.Format("SELECT {0} FROM autompg;", table);
                SQLiteCommand command = new SQLiteCommand(sql, Program.metaDbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    values.Add(reader[table].ToString());
                }
                IDF(table, values);
            }
        }

        private void IDF(String table, List<String> values)
        {
            
        }
    }
}
