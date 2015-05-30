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
            if (!Program.autompg.CreateDatabase())
            {
                return false;
            }

            if (!exists)
            {
                Program.dbConnection.Open();

                MainForm.WriteLine("Creating the meta database.");
                MainForm.WriteLine("Counting how many tuples there are.");

                String query = "SELECT count(*) FROM autompg;";
                SQLiteCommand command = new SQLiteCommand(query, Program.dbConnection);
                try
                {
                    SQLiteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    n = (long)reader[0];
                }
                catch (Exception e)
                {
                    MainForm.WriteLine(String.Format("Something went wrong: {0}", e));
                    return false;
                }
                Program.dbConnection.Close();
                MainForm.WriteLine(String.Format("There are {0} tuples in autompg.", n));

                return CreateTables() && FillTables();
            }

            return true;
        }

        private bool CreateTables()
        {
            Program.metaDbConnection.Open();
            String sql;

            MainForm.WriteLine("Creating all the tables.");

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
                catch (Exception e)
                {
                    // Tables probably already exists.
                    MainForm.WriteLine(String.Format("Something went wrong while creating table \"{0}\": {1}", table, e));
                    return false;
                }
            }
            Program.metaDbConnection.Close();
            MainForm.WriteLine("Created all the tables successfully.");

            return true;
        }

        public bool FillTables()
        {
            Program.metaDbConnection.Open();
            Program.dbConnection.Open();

            MainForm.WriteLine("Filling the meta database...");

            if (!GetQFValues())
            {
                return false;
            }

            foreach (String table in tables)
            {
                MainForm.WriteLine(String.Format("Retrieving all the values of attribute {0}.", table));

                String sql = String.Format("SELECT {0} FROM autompg;", table);
                SQLiteCommand command = new SQLiteCommand(sql, Program.dbConnection);

                Dictionary<string, double> strings = new Dictionary<string, double>();
                Dictionary<double, double> values = new Dictionary<double, double>();
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
                            double d = Convert.ToDouble(reader[0]);

                            if (values.ContainsKey(d))
                                values[d]++;
                            else
                                values.Add(d, 1);
                        }
                    }
                }
                catch (Exception e)
                {
                    MainForm.WriteLine(String.Format("Something went wrong while retrieving table \"{0}\": {1}", table, e));
                    return false;
                }

                Dictionary<double, double> newValues = GetIDFValues(values);

                MainForm.WriteLine(String.Format("Inserting the QF and IDF values into {0}.", table));
                String insert;

                foreach (KeyValuePair<double, double> entry in newValues)
                {
                    if (qfvalues.ContainsKey(table))
                    {
                        string idf = "'" + entry.Value.ToString() + "'";
                        string qf = "0";
                        if (qfvalues[table].ContainsKey(entry.Key))
                            qf = "'" + qfvalues[table][entry.Key].ToString() + "'";
                        insert = String.Format("INSERT INTO {0} VALUES ('{1}'," + idf + ", " + qf + ");", table, entry.Key);
                    }
                    else
                    {
                        string idf = "'" + entry.Value.ToString() + "'";
                        string qf = "0";
                        insert = String.Format("INSERT INTO {0} VALUES ('{1}'," + idf + ", " + qf + ");", table, entry.Key);
                    }

                    SQLiteCommand insertCommand = new SQLiteCommand(insert, Program.metaDbConnection);
                    try
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        // Value is not unique, but we dont care.
                        MainForm.WriteLine(String.Format("Something went wrong while inserting into table \"{0}\": {1}", table, e));
                    }
                }

                foreach(KeyValuePair<string, double> entry in strings)
                {
                    string idf = "'" + Math.Log10(n / entry.Value).ToString() + "'";
                    string qf = "0";
                    if (qfstrings.ContainsKey(table))
                        if (qfstrings[table].ContainsKey(entry.Key))
                            qf = "'" + qfstrings[table][entry.Key].ToString() + "'";
                    insert = String.Format("INSERT INTO {0} VALUES ('{1}'," + idf + ", " + qf + ");", table, entry.Key);

                    SQLiteCommand insertCommand = new SQLiteCommand(insert, Program.metaDbConnection);
                    try
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        // Value is not unique, but we dont care.
                        MainForm.WriteLine(String.Format("Something went wrong while inserting into table \"{0}\": {1}", table, e));
                    }
                }
            }

            Program.metaDbConnection.Close();
            Program.dbConnection.Close();

            MainForm.WriteLine("Done creating meta database.");

            return true;
        }

        private bool GetQFValues()
        {
            MainForm.WriteLine("Calculating the QF values.");

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
            MainForm.WriteLine("Trying to read the workload.txt file...");
            StreamReader reader;
            try
            {
                reader = new StreamReader("workload.txt");
            }
            catch (Exception e)
            {
                MainForm.WriteLine(String.Format("Something went wrong: {0}", e));
                return false;
            }

            String line = reader.ReadLine();
            MainForm.WriteLine("Parsing every line of the workload.txt file.");

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
            MainForm.WriteLine("Calculating the QF scores form the occurences.");

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

            MainForm.WriteLine("Done calculating the QF values.");
            return true;
        }

        private Dictionary<double, double> GetIDFValues(Dictionary<double, double> values)
        {
            //compute idf.
            MainForm.WriteLine("Computing all the IDF values.");
            Dictionary<double, double> newValues = new Dictionary<double, double>();

            double total = 0; //for each table the total value (for average and standard deviation)
            double n = 0;   //the amount of entrys (395)

            foreach (KeyValuePair<double, double> number in values) //get the values
            {
                total += number.Key * number.Value;
                n += number.Value;
            }

            double average = total / n; //calculate average

            total = 0; //now used for standard deviation

            //get the standard deviation
            foreach (KeyValuePair<double, double> number in values)
            {
                total += number.Value * Math.Abs(number.Key - average);
            }

            double standardDev = total / n; //and devide, that fun
            double h = 1.06 * standardDev * Math.Pow(n, -0.2); //get h

            foreach (KeyValuePair<double, double> number in values)
            {
                double idfValue = 0; //this is the value under de streep we are calculating
                foreach (KeyValuePair<double, double> number2 in values)
                {
                    idfValue += Math.Pow(Math.E, -0.5 * Math.Pow((number2.Key - number.Key) / h, 2));
                }

                idfValue = Math.Log10(n / idfValue); //and for the full function
                newValues.Add(number.Key, idfValue); //and add it to the correct dictionary
            }
            MainForm.WriteLine("Done calculating the IDF values.");

            return newValues;
        }
    }
}
