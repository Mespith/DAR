using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace DARpracticum
{
    static class Program
    {
        public static SQLiteConnection dbConnection;
        public static SQLiteConnection metaDbConnection;
        public static Database autompg;
        public static MetaDatabase metaDB;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // If the file does not exist, we need to create the tables and fill them as well.
            if (!File.Exists("autompg.sqlite"))
            {
                SQLiteConnection.CreateFile("autompg.sqlite");
                autompg = new Database(false);
            }
            else
            {
                autompg = new Database(true);
            }
            dbConnection = new SQLiteConnection("Data Source=autompg.sqlite;Version=3;");

            if (!File.Exists("metaDataDb.sqlite"))
            {
                SQLiteConnection.CreateFile("metaDataDb.sqlite");
                metaDB = new MetaDatabase(false);
            }
            else
            {
                metaDB = new MetaDatabase(true);
            }
            metaDbConnection = new SQLiteConnection("Data Source=metaDataDb.sqlite;Version=3;");

            Application.Run(new MainForm());

        }
    }
}
