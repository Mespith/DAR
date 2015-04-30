using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DARpracticum
{
    static class Program
    {
        public static SQLiteConnection dbConnection;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            SQLiteConnection.CreateFile("TestDatabase.sqlite");

            dbConnection = new SQLiteConnection("Data Source=TestDatabase.sqlite;Version=3;");
        }
    }
}
