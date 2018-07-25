using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiFaceRec
{
    public partial class Authen : Form
    {
        public Authen()
        {
            InitializeComponent();
        }
        [System.Runtime.InteropServices.DllImport("advapi32.dll")]
        public static extern bool LogonUser(string userName, string domainName, string password, int LogonType, int LogonProvider, ref IntPtr phToken);

        private void button1_Click(object sender, EventArgs e)
        {
            bool issuccess = false;
            string username = GetloggedinUserName();

            // if (username.ToLowerInvariant().Contains(txtUserName.Text.Trim().ToLowerInvariant()) && username.ToLowerInvariant().Contains(txtDomain.Text.Trim().ToLowerInvariant()))
            //  {
            //      issuccess = IsValidateCredentials(txtUserName.Text.Trim(), txtPwd.Text.Trim(), txtDomain.Text.Trim());
            //  }
            issuccess = IsValidateCredentials(textBox1.Text.Trim(), textBox2.Text.Trim(), textBox3.Text.Trim());
            if (issuccess)
            {
                MessageBox.Show("Successfuly Logged in !!!");
                AddMain frm = new AddMain();
                frm.Show();
                this.Close();
            }
            else
                MessageBox.Show("User Name / Password is invalid !!!");
        }
        private string GetloggedinUserName()
        {
            System.Security.Principal.WindowsIdentity currentUser = System.Security.Principal.WindowsIdentity.GetCurrent();
            return currentUser.Name;
        }

        private bool IsValidateCredentials(string userName, string password, string domain)
        {
            IntPtr tokenHandler = IntPtr.Zero;
            bool isValid = LogonUser(userName, domain, password, 2, 0, ref tokenHandler);
            return isValid;
        }
    }
}
