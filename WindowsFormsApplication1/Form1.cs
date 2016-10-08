using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = PathTextBox.Text;
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                PathTextBox.Text = folderBrowserDialog1.SelectedPath;
                OutputTextBox.Text = new GitRepositoryLogger(PathTextBox.Text).GetLog();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OutputTextBox.Text = new GitRepositoryLogger(PathTextBox.Text).GetLog();
        }
    }
}
