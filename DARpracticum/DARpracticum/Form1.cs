using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DARpracticum
{
    public partial class Form1 : Form
    {
        Database database;
        MetaDatabase metaDatabase;

        public Form1()
        {
            InitializeComponent();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            database = new Database();
        }

        private void fillButton_Click(object sender, EventArgs e)
        {
            database.FillDB();
        }

        private void createMetaData_Click(object sender, EventArgs e)
        {
            metaDatabase = new MetaDatabase();
        }

        private void fillMetaDatabase_Click(object sender, EventArgs e)
        {
            metaDatabase.FillTables();
        }
    }
}
