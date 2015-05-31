using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryHandler
{
    class Handler
    {
        private List<String> attributes;
        private Dictionary<string, string> conditions;
        private List<Auto> result;
        private int k = 10;

        public Handler()
        {
            attributes = new List<string>();
            conditions = new Dictionary<string, string>();
            result = new List<Auto>();
        }

        public List<Auto> HandleQuery(String query)
        {
            ParseQuery(query);
            ExecuteQuery();

            result.OrderBy(v => v.Similarity);
            if (result.Count < k)
            {

            }

            return result.Take(k).ToList();
        }

        private void ParseQuery(String query)
        {
            List<char> result = query.ToList();
            result.RemoveAll(c => c == ' ');
            query = new string(result.ToArray());

            String[] words = query.Split(',');

            foreach (String word in words)
            {
                String[] values = word.Split('=');
                if (values[0] == "k" || values[0] == "k ")
                {
                    k = int.Parse(values[1]);
                }
                else
                {                    
                    conditions.Add(values[0], values[1].Split(';')[0]);
                }
            }
        }        

        private void ExecuteQuery()
        {
            Program.dbConnection.Open();

            Int32 count = 0;
            String query = "SELECT * FROM autompg WHERE";
            foreach(KeyValuePair<string, string> condition in conditions)
            {
                if (count == 0)
                {
                    if (condition.Key == "type" || condition.Key == "model" || condition.Key == "brand")
                    {
                        query = String.Format("{0} {1} = {2}", query, condition.Key, condition.Value);
                    }
                    else
                    {
                        query = String.Format("{0} {1} = '{2}'", query, condition.Key, condition.Value);
                    }
                }
                else if (count == conditions.Count - 1)
                {
                    if (condition.Key == "type" || condition.Key == "model" || condition.Key == "brand")
                    {
                        query = String.Format("{0} AND {1} = {2};", query, condition.Key, condition.Value);
                    }
                    else
                    {
                        query = String.Format("{0} AND {1} = '{2}';", query, condition.Key, condition.Value);
                    }
                }
                else
                {
                    if (condition.Key == "type" || condition.Key == "model" || condition.Key == "brand")
                    {
                        query = String.Format("{0} AND {1} = {2}", query, condition.Key, condition.Value);
                    }
                    else
                    {
                        query = String.Format("{0} AND {1} = '{2}'", query, condition.Key, condition.Value);
                    }
                }
                count++;
            }
            SQLiteCommand command = new SQLiteCommand(query, Program.dbConnection);
            SQLiteDataReader reader;

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                throw e;
            }

            while(reader.Read())
            {
                Auto auto = new Auto();
                
                foreach(string table in Program.tables)
                {
                    auto.values.Add(table, reader[table]);
                }
                auto.Calculatesimilarity(conditions);

                result.Add(auto);
            }

            Program.dbConnection.Close();
        }
    }
}
