using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MultiFaceRec
{
    public partial class DelSub : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\project working\5\ClassEye\Database1.mdf;Integrated Security=True");
        public DelSub()
        {
            InitializeComponent();
            con.Open();
            bind();
            MessageBox.Show("Student Name and there Attendence Addede to the selected subject will also be deleted");
        }

        ~DelSub()
        {
            //Close DataBase Connection
            con.Close();
        }
        private void bind()
        {

            SqlDataAdapter da = new SqlDataAdapter("Select id,sub_name from subject", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataRow dr;
            dr = dt.NewRow();
            dr.ItemArray = new object[] { 0, "---Select an Subject---" };
            dt.Rows.InsertAt(dr, 0);
            //comboBox1.DisplayMember = "sub_name";
            //comboBox1.ValueMember = "id";
            //comboBox1.DataSource = dt;
            if (dt != null)
            {
                comboBox1.DisplayMember = dt.Columns[1].ToString();
                comboBox1.ValueMember = dt.Columns[0].ToString();
                comboBox1.DataSource = dt;

            }
            else
            {
                MessageBox.Show("No subject added to database .Before proceeding add subjects.");
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                string cb = "";
                var row = (DataRowView)comboBox1.SelectedItem;
                //cb = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                cb = row["sub_name"].ToString();
                string SCom1 = "delete from subject where sub_name ='" + cb + "'";
                //Database Command 1
                SqlCommand DBcom1 = new SqlCommand(SCom1, con);

                //Execute Database Command 1.
                DBcom1.ExecuteNonQuery();

                //Dispose Database Command 1
                DBcom1.Dispose();
                string SCom2 = "delete from student where subject = '" + cb + "'";
                //Database Command 2
                SqlCommand DBcom2 = new SqlCommand(SCom2, con);

                //Execute Database Command 2.
                DBcom2.ExecuteNonQuery();

                //Dispose Database Command 2
                DBcom2.Dispose();
                MessageBox.Show("subject = "+ cb +" Deleted ");
            }
            else
                MessageBox.Show("Please Select Subject", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
     }
}
