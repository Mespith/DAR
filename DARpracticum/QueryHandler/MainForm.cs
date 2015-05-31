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

namespace QueryHandler
{
    public partial class MainForm : Form
    {
        // That's our custom TextWriter class
        TextWriter _writer = null;
        Handler queryHandler;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Instantiate the writer
            _writer = new TextBoxStreamWriter(resultBox);
            // Redirect the out Console stream
            Console.SetOut(_writer);

            Console.WriteLine("The results of your query will be shown here.");

            queryHandler = new Handler();
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                // Handle the query.
                List<Auto> results = queryHandler.HandleQuery(inputBox.Text);

                for (int i = 0; i < results.Count; i++)
                {
                    Console.WriteLine(String.Format("{0}. {1}.", i + 1, results[i].ToString()));
                }

                inputBox.Clear();
                e.Handled = true;
            }
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
