using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using ClosedXML.Excel;

namespace MultiFaceRec
{
    public partial class Display : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\project working\5\ClassEye\Database1.mdf;Integrated Security=True");
        public Display()
        {
            InitializeComponent();
            con.Open();
            bind();
        }

        ~Display()
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

        private void button2_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            if (comboBox1.SelectedIndex > 0)
            {
                string cb = "";
                var row = (DataRowView)comboBox1.SelectedItem;
                //cb = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                cb = row["sub_name"].ToString();
                string constring = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\project working\5\ClassEye\Database1.mdf;Integrated Security=True";
                using (SqlConnection con1 = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM student where subject ='" + cb + "'", con1))
                    {
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                dataGridView1.DataSource = dt;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Select Subject", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                printDocument1.Print();
            }
            else
                MessageBox.Show("No Data To Print");
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap dataGridViewImage = new Bitmap(this.dataGridView1.Width, this.dataGridView1.Height);
            dataGridView1.DrawToBitmap(dataGridViewImage, new Rectangle(0, 0, this.dataGridView1.Width, this.dataGridView1.Height));
            e.Graphics.DrawImage(dataGridViewImage, 0, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (dataGridView1.Rows.Count > 0)
            {
                try
                {
                    // Bind Grid Data to Datatable
                    DataTable dt = new DataTable();
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        dt.Columns.Add(col.HeaderText, col.ValueType);
                    }
                    int count = 0;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (count < dataGridView1.Rows.Count - 1)
                        {
                            dt.Rows.Add();
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                dt.Rows[dt.Rows.Count - 1][cell.ColumnIndex] = cell.Value.ToString();
                            }
                        }
                        count++;
                    }
                    // Bind table data to Stream Writer to export data to respective folder
                    StreamWriter wr = new StreamWriter(@"c:\\attendence.xls");
                    // Write Columns to excel file
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        wr.Write(dt.Columns[i].ToString().ToUpper() + "\t");
                    }
                    wr.WriteLine();
                    //write rows to excel file
                    for (int i = 0; i < (dt.Rows.Count); i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (dt.Rows[i][j] != null)
                            {
                                wr.Write(Convert.ToString(dt.Rows[i][j]) + "\t");
                            }
                            else
                            {
                                wr.Write("\t");
                            }
                        }
                        wr.WriteLine();
                    }
                    wr.Close();
                    MessageBox.Show("Data Exported Successfully");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                MessageBox.Show("No Data to export into excel");
        }
    }
    
}
