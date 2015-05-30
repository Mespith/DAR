using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARpracticum
{
    class MetaDatabase
    {
        Dictionary<string, Dictionary<string, double>> qfstrings = new Dictionary<string, Dictionary<string, double>>();
        Dictionary<string, Dictionary<double, double>> qfvalues = new Dictionary<string, Dictionary<double, double>>();

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

        private bool exists;
        private long n;

        public MetaDatabase(bool _exists)
        {
            exists = _exists;
        }

        public bool CreateDatabase()
        {
            Program.autompg.CreateDatabase();

            if (!exists)
            {
                Program.dbConnection.Open();

                String query = "SELECT count(*) FROM autompg;";
                SQLiteCommand command = new SQLiteCommand(query, Program.dbConnection);
                try
                {
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    n = (long)reader[0];
                }
                catch
                {
                    return false;
                }
                Program.dbConnection.Close();

                return CreateTables() && FillTables();
            }

            return true;
        }

        private bool CreateTables()
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
                    return false;
                }
            }

            Program.metaDbConnection.Close();
            return true;
        }

        public bool FillTables()
        {
            Program.metaDbConnection.Open();
            Program.dbConnection.Open();

            GetQFValues();

            foreach (String table in tables)
            {
                String sql = String.Format("SELECT {0} FROM autompg;", table);
                SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);

                Dictionary<string, double> strings = new Dictionary<string, double>();
                Dictionary<string, Dictionary<double, double>> values = new Dictionary<string, Dictionary<double, double>>();
                SQLiteDataReader reader;

                try
                {
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (table == "brand" || table == "type" || table == "model")
                        {

                            string i = (string)reader[0];
                            if (strings.ContainsKey(i))
                                strings[i]++;
                            else
                                strings.Add(i, 1);
                        }
                        else
                        {
                            if (!values.ContainsKey(table))
                                values.Add(table, new Dictionary<double, double>());
                            double d = Convert.ToDouble(reader[0]);

                            if (values[table].ContainsKey(d))
                                values[table][d]++;
                            else
                                values[table].Add(d, 1);
                        }
                    }
                }
                catch
                {
                    return false;
                }

                Dictionary<string, Dictionary<double, double>> newValues = GetIDFValues(values, table);

                command = new SQLiteCommand(sql, Program.dbConnection);
                try
                {
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        String insert;

                        if (table == "brand" || table == "type" || table == "model")
                        {
                            string idf = "'" + Math.Log10(395 / strings[(string)reader[0]]).ToString() + "'";
                            string qf = "0";
                            string test = (string)reader[0];
                            if (qfstrings.ContainsKey(table))
                                if (qfstrings[table].ContainsKey((string)reader[0]))
                                    qf = "'" + qfstrings[table][(string)reader[0]].ToString() + "'";
                            insert = String.Format("INSERT INTO {0} VALUES ('{1}'," + idf + ", " + qf + ");", table, reader[table]);
                        }
                        else if (qfvalues.ContainsKey(table))
                        {
                            string idf = "'" + newValues[table][(Convert.ToDouble(reader[0]))].ToString() + "'";
                            string qf = "0";
                            if (qfvalues[table].ContainsKey(Convert.ToDouble(reader[0])))
                                qf = "'" + qfvalues[table][Convert.ToDouble(reader[0])].ToString() + "'";
                            insert = String.Format("INSERT INTO {0} VALUES ('{1}'," + idf + ", " + qf + ");", table, reader[table]);
                        }
                        else
                        {
                            string idf = "'" + newValues[table][(Convert.ToDouble(reader[0]))].ToString() + "'";
                            string qf = "0";
                            insert = String.Format("INSERT INTO {0} VALUES ('{1}'," + idf + ", " + qf + ");", table, reader[table]);
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
                catch (Exception e)
                {
                    Form1.WriteLine(String.Format("Something went wrong: {0}", e));
                    return false;
                }
            }

            Program.metaDbConnection.Close();
            Program.dbConnection.Close();

            return true;
        }

        private void GetQFValues()
        {
            //for jacuard:
            double[,] brandJac = new double[29, 29];
            double[,] typeJac = new double[6, 6];
            string[] brandNames = new string[29];
            string[] typeNames = new string[6];

            // dictionarys for calculating qf scores
            Dictionary<string, Dictionary<string, double>> oldqfstrings = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<double, double>> oldqfvalues = new Dictionary<string, Dictionary<double, double>>();
            qfstrings = new Dictionary<string, Dictionary<string, double>>();
            qfvalues = new Dictionary<string, Dictionary<double, double>>();

            //read workload
            StreamReader reader = new StreamReader("workload.txt");
            String line = reader.ReadLine();

            while (!String.IsNullOrEmpty(line))
            {
                string[] words = line.Split();
                //#occurences of 1 query
                int times = Convert.ToInt16(words[0]);

                //the first 7 words are not interesting, we are calculating the amount of occurences of each attribute
                for (int i = 7; i < words.Count(); i++)
                {
                    //add non numerical values.
                    if ((words[i] == "brand" && words[i + 1] != "IN") || (words[i] == "type" && words[i + 1] != "In") || words[i] == "model")
                    {
                        string value = words[i + 2];
                        if (!oldqfstrings.ContainsKey(words[i]))
                            oldqfstrings.Add(words[i], new Dictionary<string, double>());
                        if (oldqfstrings[words[i]].ContainsKey(value))
                            oldqfstrings[words[i]][value] += times;
                        else
                            oldqfstrings[words[i]].Add(value, times);
                    }
                    //add numerical values
                    else if (words[i] == "mpg" || words[i] == "cylinders" || words[i] == "displacement" || words[i] == "horsepower" ||
                        words[i] == "weight" || words[i] == "acceleration" || words[i] == "model_year" || words[i] == "origin")
                    {
                        double d = Convert.ToDouble(words[i + 2]);
                        if (!oldqfvalues.ContainsKey(words[i]))
                            oldqfvalues.Add(words[i], new Dictionary<double, double>());
                        if (oldqfvalues[words[i]].ContainsKey(d))
                            oldqfvalues[words[i]][d] += times;
                        else
                            oldqfvalues[words[i]].Add(d, times);
                    }
                    else if ((words[i] == "brand" || words[i] == "type") && words[i + 1] == "IN")
                    {
                        string value = words[i + 1];

                    }
                }

                line = reader.ReadLine();
            }

            //calculating qf scores from the occurences
            foreach (KeyValuePair<string, Dictionary<string, double>> table in oldqfstrings)
            {
                qfstrings.Add(table.Key, new Dictionary<string, double>());
                //determing the max of 1 attribute
                double max = 0;
                foreach (KeyValuePair<string, double> number in table.Value)
                {
                    if (number.Value > max)
                        max = number.Value;
                }
                //save the qf scores in dictionary for non numerical
                foreach (KeyValuePair<string, double> number in table.Value)
                {
                    double x = number.Value;
                    qfstrings[table.Key].Add(number.Key, x / max);
                }
            }

            foreach (KeyValuePair<string, Dictionary<double, double>> table in oldqfvalues)
            {
                qfvalues.Add(table.Key, new Dictionary<double, double>());
                //determing max of 1 attribute.
                double max = 0;
                foreach (KeyValuePair<double, double> number in table.Value)
                {
                    if (number.Value > max)
                        max = number.Value;
                }
                //save correct qf values.
                foreach (KeyValuePair<double, double> number in table.Value)
                {
                    double x = number.Value;
                    qfvalues[table.Key].Add(number.Key, x / max);
                }
            }

        }

        private Dictionary<string, Dictionary<double, double>> GetIDFValues(Dictionary<string, Dictionary<double, double>> values, String table)
        {
            Dictionary<string, Dictionary<double, double>> newValues = new Dictionary<string, Dictionary<double, double>>();
            //compute idf.
            foreach (KeyValuePair<string, Dictionary<double, double>> t in values)
            {
                newValues.Add(table, new Dictionary<double, double>());
                double total = 0; //for each table the total value (for average and standard deviation)
                double n = 0;   //the amount of entrys (395)

                foreach (KeyValuePair<double, double> number in t.Value) //get the values
                {
                    total += number.Key * number.Value;
                    n += number.Value;
                }

                double average = total / n; //calculate average

                total = 0; //now used for standard deviation

                //get the standard deviation
                foreach (KeyValuePair<double, double> number in t.Value)
                {
                    total += number.Value * Math.Abs(number.Key - average);
                }

                double standardDev = total / n; //and devide, that fun
                double h = 1.06 * standardDev * Math.Pow(n, -0.2); //get h

                foreach (KeyValuePair<double, double> number in t.Value)
                {
                    double idfValue = 0; //this is the value under de streep we are calculating
                    foreach (KeyValuePair<double, double> number2 in t.Value)
                    {
                        idfValue += Math.Pow(Math.E, -0.5 * Math.Pow((number2.Key - number.Key) / h, 2));
                    }

                    idfValue = Math.Log10(n / idfValue); //and for the full function
                    newValues[table].Add(number.Key, idfValue); //and add it to the correct dictionary
                }
            }

            return newValues;
        }
    }
}
