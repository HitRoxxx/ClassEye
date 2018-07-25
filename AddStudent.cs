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
    public partial class AddStudent : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\project working\5\ClassEye\Database1.mdf;Integrated Security=True");
        public AddStudent()
        {
            InitializeComponent();
            con.Open();
            bind();
        }

        ~AddStudent()
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            int count = 0;
           
            for (i = 0; i < textBox2.Lines.Length; i++)
            {
                if (string.Compare(textBox2.Lines[i], "") != 0)
                {
                    try
                    {
                        count++;
                        label4.Text = count.ToString();
                    }
                  
                    catch (System.OverflowException)
                    {

                        textBox2.Lines[i].Remove(0);
                        count = 0;
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                string cb = "";
                int a = 0,i=0,j=0;
                var row = (DataRowView)comboBox1.SelectedItem;
                //cb = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                cb = row["sub_name"].ToString();

                int IRTBLines = 0; // Counter Represent Lines from RichTextBox
                                   //Read Lines from RichTextBox
                for (IRTBLines = 0; IRTBLines < textBox2.Lines.Length; IRTBLines++)
                {
                   
                    //If IRTBLine is not Null Then
                    if (string.Compare(textBox2.Lines[IRTBLines], "") != 0)
                    {
                        //Data Base Command 2
                        SqlCommand DBcom2 = new SqlCommand("select * from student where subject = '" + cb + "' AND student_name ='" + textBox2.Lines[IRTBLines] + "'", con);

                        //Database Reader 2                    
                        SqlDataReader DBRead2;

                        //Read From Database
                        DBRead2 = DBcom2.ExecuteReader();

                        // Check if Department Name is Exist
                        if (DBRead2.HasRows)
                        {
                            j = 1;
                            //Department Name is Already Exist
                            MessageBox.Show("student Name =" + textBox2.Lines[IRTBLines] + " is skipped because it is already exist");

                            //Dispose Database command 2 and reader 2
                            DBRead2.Dispose();
                            DBcom2.Dispose();
                        }
                        else
                        {
                            i = 1;
                            //Department Name is Not Already Exist                                                                       
                            //Database Command3 String
                            string SCom1 = "insert into student(subject,student_name,attendence) values('" + cb + "','" + textBox2.Lines[IRTBLines].ToString() + "','" + a + "')";
                            //Dispose Databse command 2 and reader 2
                            DBRead2.Dispose();
                            DBcom2.Dispose();

                            //Database Command 3
                            SqlCommand DBcom3 = new SqlCommand(SCom1, con);

                            //Execute Database Command 3.
                            DBcom3.ExecuteNonQuery();

                            //Dispose Database Command 3
                            DBcom3.Dispose();

                           
                        }
                      //  MessageBox.Show("Data Entered Into Database");

                    }
                   
                }
                if(i==0 && j==0)
                {
                    MessageBox.Show("Please input name in text box", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                if(i==1)
                    MessageBox.Show("Data Entered Into Database");

                //Clear RichTextBox1 Content
                textBox2.ResetText();
                label4.Text = "0";
            }
            else
            {
                MessageBox.Show("Please Select Subject", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
            }

        }
    }
}

