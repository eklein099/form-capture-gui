using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form_Capture_GUI
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            
        }

        public void populate_dropdown()
        {
            List<string> fields = Program.get_fields();
            comboBox1.DataSource = fields;
            textBox4.Text = Program.perm_token;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += textBox3.Text+"\r\n";
            textBox2.Text += "" + comboBox1.SelectedItem + "\r\n";
            Program.fieldNames.Add((""+comboBox1.SelectedItem));
            Program.formNames.Add(("" + textBox3.Text));
            textBox3.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string output = Program.generate_script(Program.fieldNames,textBox4.Text);
            FormPopUp formPopUp = new FormPopUp();
            formPopUp.setContent(output);
            formPopUp.Show();
        }
    }
}
