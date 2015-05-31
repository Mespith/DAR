﻿using System;
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
                            if (query[value.Key] == value.Value)
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
                                    similarity += (double)reader["qfidf"];
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
                            double qfidf = Convert.ToDouble(readerQ["qfidf"].ToString()); //this is the value under de streep we are calculating
                            double t = double.Parse(value.Value.ToString());
                            double q = double.Parse(query[value.Key]);
                            double power = -0.5 * Math.Pow((t - q) / h, 2);

                            similarity += Math.Pow(Math.E, power) * qfidf;
                        }
                    }
                }
                Similarity = similarity;
                Program.metaDbConnection.Close();
            }
        }
    }
}