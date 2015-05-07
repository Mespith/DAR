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

            SQLiteConnection.CreateFile("metadatadb.sqlite");
            dbConnection = new SQLiteConnection("Data Source=metadatadb.sqlite;Version=3;");

            Application.Run(new Form1());

        }
    }
}
