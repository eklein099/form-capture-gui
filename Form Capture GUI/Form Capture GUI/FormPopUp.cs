using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Form_Capture_GUI
{
    public partial class FormPopUp : Form
    {
        public FormPopUp()
        {
            InitializeComponent();
        }
        public void setContent(string content)
        {
            textBox1.Text = content;
        }
    }
}
