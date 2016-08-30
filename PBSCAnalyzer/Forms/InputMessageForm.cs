using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBSCAnalyzer.Forms
{
    public partial class InputMessageForm : Form
    {
        public InputMessageForm()
        {
            InitializeComponent();
            checkBox1.Checked = true;
            checkBox2.Checked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (App.Configuration.WorkSpaces.Any(x => x.Name == textBox1.Name))
            {
                MessageBox.Show($"Workspace [{textBox1.Text}] exisits!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void InputMessageForm_Load(object sender, EventArgs e)
        {

        }
    }
}
