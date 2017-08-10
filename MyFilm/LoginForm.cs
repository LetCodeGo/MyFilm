using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void comboBoxDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonString.DataBaseName = this.comboBoxDataBase.SelectedItem.ToString();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.comboBoxDataBase.SelectedIndex = 0;
            this.comboBoxProcessCommct.SelectedIndex = 0;
            this.Icon = Properties.Resources.ico;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.textBoxUser.Text != "root" || this.textBoxPwd.Text != "123456")
            {
                MessageBox.Show("用户名或密码不正确！", "提示", MessageBoxButtons.OK);
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void comboBoxProcessCommct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessCommunication.processCommunicateType = 
                (ProcessCommunication.ProcessCommunicationType)(this.comboBoxProcessCommct.SelectedIndex);
        }
    }
}
