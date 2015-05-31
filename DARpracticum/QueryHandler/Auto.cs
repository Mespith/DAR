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

        public void Calculatesimilarity(Dictionary<string, string>query)
        {
            if (Similarity == null)
            {
                double similarity = 0;
                Program.metaDbConnection.Open();

                foreach (KeyValuePair<string, object> value in values)
                {
                    // You can only compare the query with the values if the query contains that value.
                    if (query.ContainsKey(value.Key.ToString()))
                    {
                        // Check if it is categorical information.
                        if (value.Key == "brand" || value.Key == "type" || value.Key == "model")
                        {
                            // Only calculate similarity if the value corresponds to the query.
                            if (query[value.Key].Equals(value.Value.ToString()))
                            {
                                String selectQuery;
                                selectQuery = String.Format("SELECT * FROM {0} WHERE waarde = '{1}';", value.Key, value.Value);

                                SQLiteCommand command = new SQLiteCommand(selectQuery, Program.metaDbConnection);
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
                                    similarity += (double)reader["qf"] * (double)reader["idf"];
                                }
                            }
                            else if (value.Key == "brand" || value.Key == "type")
                            {
                                String selectJaccardQuery = String.Format("SELECT * FROM jaccard WHERE waarde1 IN ('{0}',{1}) AND waarde2 IN ('{0}',{1});;", value.Value, query[value.Key]);
                                String selectQuery = String.Format("SELECT * FROM {0} WHERE waarde = {1};", value.Key, query[value.Key]);

                                SQLiteCommand commandJ = new SQLiteCommand(selectJaccardQuery, Program.metaDbConnection);
                                SQLiteCommand commandQF = new SQLiteCommand(selectQuery, Program.metaDbConnection);
                                SQLiteDataReader readerJ;
                                SQLiteDataReader readerQF;

                                try
                                {
                                    readerJ = commandJ.ExecuteReader();
                                    readerQF = commandQF.ExecuteReader();
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }

                                if (readerJ.Read() && readerQF.Read())
                                {
                                    similarity += (double)readerJ["coefficient"] * (double)readerQF["qf"];
                                }
                            }
                        }
                        // It is numerical information.
                        else
                        {
                            String selectQueryT = String.Format("SELECT * FROM {0} WHERE waarde = '{1}';", value.Key, value.Value);
                            String selectQueryQ = String.Format("SELECT * FROM {0} WHERE waarde = '{1}';", value.Key, query[value.Key]);

                            SQLiteCommand commandT = new SQLiteCommand(selectQueryT, Program.metaDbConnection);
                            SQLiteCommand commandQ = new SQLiteCommand(selectQueryQ, Program.metaDbConnection);
                            SQLiteDataReader readerT;
                            SQLiteDataReader readerQ;

                            try
                            {
                                readerT = commandT.ExecuteReader();
                                readerQ = commandQ.ExecuteReader();
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }

                            readerT.Read();
                            readerQ.Read();

                            double h = Convert.ToDouble(readerT["h"].ToString()) / 100; //get h
                            double idf = Convert.ToDouble(readerQ["idf"].ToString()); //this is the value under de streep we are calculating
                            double t = double.Parse(value.Value.ToString());
                            double q = double.Parse(query[value.Key]);
                            double power = -0.5 * Math.Pow((t - q) / h, 2);

                            similarity += Math.Pow(Math.E, power) * idf;
                        }
                    }
                }
                Similarity = similarity;
                Program.metaDbConnection.Close();
            }
        }

        public override string ToString()
        {
            String result = "Car: ";
            foreach(KeyValuePair<string, object> value in values)
            {
                result = String.Format("{0} {1}: {2}; ", result, value.Key, value.Value);
            }

            return result;
        }
    }
}
