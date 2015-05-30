using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DARpracticum
{
    public partial class Form1 : Form
    {
        // That's our custom TextWriter class
        TextWriter _writer = null;

        Database database;
        MetaDatabase metaDatabase;

        public Form1()
        {
            InitializeComponent();
        }

        public static void WriteLine(String message)
        {
            Console.WriteLine(message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Instantiate the writer
            _writer = new TextBoxStreamWriter(txtConsole);
            // Redirect the out Console stream
            Console.SetOut(_writer);

            Console.WriteLine("Now redirecting output to the text box");
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            Program.metaDB.CreateDatabase();
        }
    }

    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString()); // When character data is written, append it to the text box.
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
