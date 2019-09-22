using System;
using System.Windows.Forms;

namespace Weekly_Schedule
{
    public partial class Updater : Form
    {
        public Updater()
        {
            InitializeComponent();
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.label1.Location = new System.Drawing.Point(this.progressBar1.Size.Width, 4);
            this.Size = new System.Drawing.Size(this.progressBar1.Size.Width + this.label1.Size.Width, this.progressBar1.Size.Height);
        }

        private void callback(string item)
        {


        }

        private void Updater_Shown(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            CourseManager.GetAllCourses(callback);
            CourseManager.Serialize();
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {

                var code = CourseManager.GetDownloadingCode();

                try
                {
                    if (CourseManager.CourseCodeList.Count > 0)
                    {
                        int i = CourseManager.CourseCodeList.FindIndex(item => item == code) + 1;
                        int progress = (int)(((double)i / (double)CourseManager.CourseCodeList.Count) * 100.0);
                        Console.Write("Downloading ");
                        Console.Write(progress);
                        Console.WriteLine(" / 100");
                        backgroundWorker1.ReportProgress(progress);
                    }



                    //string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    //if (code != "")
                    //{
                    //    int j = alphabet.IndexOf(code[0]) + 1;
                    //    backgroundWorker1.ReportProgress((int)(((float)j / (float)alphabet.Length) * 100.0));
                    //}
                }
                catch (Exception ex)
                {
                    Console.Write("EXXX: ");
                    Console.WriteLine(ex);
                }

                label1.Invoke((MethodInvoker)delegate
                {
                    label1.Text = code;
                });
            }
        }

        private void Updater_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker2.CancelAsync();
            backgroundWorker1.CancelAsync();

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
    }
}
