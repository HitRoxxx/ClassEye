using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;


namespace MultiFaceRec
{
    public partial class FrmP : Form
    {
        //Declararation of all variables, vectors and haarcascades
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
       // HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels= new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\project working\5\ClassEye\Database1.mdf;Integrated Security=True");

      

        public FrmP()
        {
            InitializeComponent();
            button2.Enabled = false;
            con.Open();
            bind();
            //Load haarcascades for face detection
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            //eye = new HaarCascade("haarcascade_eye.xml");
            try
            {
                //Load of previus trainned faces and labels for each image
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels+1; tf++)
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));
                    labels.Add(Labels[tf]);
                    
                }
            
            }
            catch(Exception e)
            {
               
                MessageBox.Show("Nothing in Face database, please add at least a face(Sending To Admin section to Add Face)", "Triained faces load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //  FrmP frma = new FrmP();
                // frma.            
                // this.Close();
                this.WindowState = FormWindowState.Minimized;
                //  this.Hide();
                Authen frm = new Authen();
                frm.Show();

            }
          

        }
        ~FrmP()
        {
            //Close DataBase Connection
            con.Close();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //Initialize the capture device
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            Application.Idle += new EventHandler(FrameGrabber);
            button1.Enabled = false;
            button2.Enabled = true;
        }


        private void button2_Click(object sender, System.EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                string cb = "";
                var row = (DataRowView)comboBox1.SelectedItem;
                //cb = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                cb = row["sub_name"].ToString();
                int IRTBLines = 0; // Counter Represent Lines from RichTextBox
                                   //Read Lines from RichTextBox
                for (IRTBLines = 0; IRTBLines < textBox1.Lines.Length; IRTBLines++)
                {
                    //If IRTBLine is not Null Then
                    if (string.Compare(textBox1.Lines[IRTBLines], "") != 0)
                    {
                        //Data Base Command 2
                        SqlCommand DBcom2 = new SqlCommand("select * from student where subject = '" + cb + "' AND student_name ='" + textBox1.Lines[IRTBLines] + "'", con);

                        //Database Reader 2                    
                        SqlDataReader DBRead2;

                        //Read From Database
                        DBRead2 = DBcom2.ExecuteReader();

                        // Check if Department Name is Exist
                        if (DBRead2.HasRows)
                        {
                            //string SCom1 = "insert into student(subject,student_name,attendence) values('" + cb + "','" + textBox1.Lines[IRTBLines].ToString() + "','" + a + "')";
                            //Dispose Databse command 2 and reader 2
                            string SCom1 = "update student set attendence = attendence +1 where subject = '" + cb + "' AND student_name ='" + textBox1.Lines[IRTBLines] + "'";
                            DBRead2.Dispose();
                            DBcom2.Dispose();

                            //Database Command 3
                            SqlCommand DBcom3 = new SqlCommand(SCom1, con);

                            //Execute Database Command 3.
                            DBcom3.ExecuteNonQuery();

                            //Dispose Database Command 3
                            DBcom3.Dispose();

                        }
                        else
                        {
                            //Department Name is Already Exist
                            MessageBox.Show("student Name =" + textBox1.Lines[IRTBLines] + " These Student name not found in database (please add these student)");

                            //Dispose Database command 2 and reader 2
                            DBRead2.Dispose();
                            DBcom2.Dispose();

                        }

                    }
                }

                //Clear RichTextBox1 Content
                textBox1.ResetText();

            }
            else
            {
                MessageBox.Show("Please Select Subject", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        // above need attention
        void FrameGrabber(object sender, EventArgs e)
        {
           // label3.Text = "0";
            //label4.Text = "";
            NamePersons.Add("");


            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                    //Convert it to Grayscale
                    gray = currentFrame.Convert<Gray, Byte>();

                    //Face Detector
                    MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
                  face,
                  1.2,
                  2,
                  Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                  new Size(20, 20));

                    //Action for each element detected
                    foreach (MCvAvgComp f in facesDetected[0])
                    {
                        t = t + 1;
                        result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                        //draw the face detected in the 0th (gray) channel with blue color
                        currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                        if (trainingImages.ToArray().Length != 0)
                        {
                            //TermCriteria for face recognition with numbers of trained images like maxIteration
                        MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                        //Eigen face recognizer
                        EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                           trainingImages.ToArray(),
                           labels.ToArray(),
                           5000,
                           ref termCrit);

                        name = recognizer.Recognize(result);
                     

                    //Draw the label for each face detected and recognized
                    currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                        }

                            NamePersons[t-1] = name;
                            NamePersons.Add("");

                    }
                        t = 0;

                        //Names concatenation of persons recognized
                    for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                    {
                        names = names + NamePersons[nnn] + "\n";
                
                       // names = names.Replace("@", "@" + System.Environment.NewLine);
                      // label2.Text = names;
                      textBox1.Text = names;
                     label2.Text = textBox1.Text;
                    }
                    //Show the faces procesed and recognized
                    imageBoxFrameGrabber.Image = currentFrame;
                    //label4.Text = names;
                    names = "";
                    //Clear the list(vector) of names
                    NamePersons.Clear();
                    

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
                MessageBox.Show("No subject is added into database.Before proceed add subjects.");
                this.Hide();
            }
        }



    }
}