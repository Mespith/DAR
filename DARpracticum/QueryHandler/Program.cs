using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryHandler
{
    static class Program
    {
        public static SQLiteConnection dbConnection;
        public static SQLiteConnection metaDbConnection;
        public static String[] tables = new String[]
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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            dbConnection = new SQLiteConnection("Data Source=autompg.sqlite;Version=3;");
            metaDbConnection = new SQLiteConnection("Data Source=metaDataDb.sqlite;Version=3;");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
