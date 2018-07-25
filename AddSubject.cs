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
    public partial class AddSubject : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\project working\5\ClassEye\Database1.mdf;Integrated Security=True");
        public AddSubject()
        {
            InitializeComponent();
            con.Open();
        }

        ~AddSubject()
        {
            //Close DataBase Connection
            con.Close();
        }
    

        private void button1_Click(object sender, EventArgs e)
        {
            int IRTBLines = 0, i=0,j=0; // Counter Represent Lines from TextBox
            //Read Lines from TextBox
            for (IRTBLines = 0; IRTBLines < textBox1.Lines.Length; IRTBLines++)
            {
                //If IRTBLine is not Null Then
                if (string.Compare(textBox1.Lines[IRTBLines], "") != 0)
                {
                   
                    //Data Base Command 2
                    SqlCommand DBcom2 = new SqlCommand("select * from subject where sub_name ='" + textBox1.Lines[IRTBLines] + "'", con);

                    //Database Reader 2                    
                    SqlDataReader DBRead2;

                    //Read From Database
                    DBRead2 = DBcom2.ExecuteReader();

                    // Check if Department Name is Exist
                    if (DBRead2.HasRows)
                    {
                        j = 1;
                        //Department Name is Already Exist
                        MessageBox.Show("subject Name =" + textBox1.Lines[IRTBLines] + " is skipped because it is already exist");

                        //Dispose Database command 2 and reader 2
                        DBRead2.Dispose();
                        DBcom2.Dispose();
                    }
                    else
                    {
                        i = 1;
                        //Department Name is Not Already Exist                                                                       
                        //Database Command3 String
                        string SCom1 = "insert into subject values('" + textBox1.Lines[IRTBLines].ToString() + "')";
                        //Dispose Databse command 2 and reader 2
                        DBRead2.Dispose();
                        DBcom2.Dispose();

                        //Database Command 3
                        SqlCommand DBcom3 = new SqlCommand(SCom1, con);

                        //Execute Database Command 3.
                        DBcom3.ExecuteNonQuery();

                        //Dispose Database Command 3
                        DBcom3.Dispose();

                        //MessageBox.Show("subject Name =" + textBox1.Lines[IRTBLines] + " is entered into database");

                    }

                }
                
            }

            if (i == 0) 
                MessageBox.Show("Please input subject in text box", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            if(i == 1)
                MessageBox.Show("Data Entered Into Database");
            //Clear RichTextBox1 Content
            textBox1.ResetText();
            
        }
    }
}
