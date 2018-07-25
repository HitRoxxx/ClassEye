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
    public partial class AddMain : Form
    {
        public AddMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.Hide();
            FrmPrincipal frm = new FrmPrincipal();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           // this.Hide();
            AddStudent frm = new AddStudent();
            frm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           // this.Hide();
            AddSubject frm = new AddSubject();
            frm.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Display frm = new Display();
            frm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DelSub frm = new DelSub();
            frm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DelStu frm = new DelStu();
            frm.Show();
        }
    }
}
