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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!File.Exists("database.sqlite"))
                SQLiteConnection.CreateFile("database.sqlite");

            if (!File.Exists("metaDataDb.sqlite"))
                SQLiteConnection.CreateFile("metaDataDb.sqlite");

            dbConnection = new SQLiteConnection("Data Source=database.sqlite;Version=3;");
            metaDbConnection = new SQLiteConnection("Data Source=metaDataDb.sqlite;Version=3;");

            Application.Run(new Form1());

        }
    }
}
