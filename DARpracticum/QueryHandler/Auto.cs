using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryHandler
{
    class Auto
    {
        public Dictionary<string, object> values;

        public double? Similarity;

        public Auto()
        {
            values = new Dictionary<string, object>();
        }

        public void Calculatesimilarity()
        {
            if (Similarity == null)
            {
                double similarity = 0;
                Program.metaDbConnection.Open();

                foreach (KeyValuePair<string, object> value in values)
                {
                    String query;
                    query = String.Format("SELECT * FROM {0} WHERE waarde = '{1}';", value.Key, value.Value);

                    SQLiteCommand command = new SQLiteCommand(query, Program.metaDbConnection);
                    SQLiteDataReader reader;

                    try
                    {
                        reader = command.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    while (reader.Read())
                    {
                        similarity += (double)reader["idf"] * (double)reader["qf"];
                    }
                }

                Similarity = similarity;
                Program.metaDbConnection.Close();
            }
        }
    }
}
