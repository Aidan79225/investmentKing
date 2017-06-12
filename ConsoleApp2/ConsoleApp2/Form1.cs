using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp2
{
    public partial class Form1 : Form
    {
        List<Company> companys = new List<Company>();
        int observation = 12;
        int holding = 1;
        int skip = 0;
        int priceSize = 4696;
        int top = 0;
        int winnerP = 30;
        string filePath = "";
        string topFilePath = "";
        string strPath = @"c:\temp\";
        string fileName = "MyTest.csv";
        public Form1()
        {
            InitializeComponent();
            initBackgroundWorker();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private BackgroundWorker bw;
        private void initBackgroundWorker()
        {
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label6.Text = "完成";
            progressBar1.Value = 100;
        }
        string type = "top";
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if (e.ProgressPercentage < 0.1)
            {
                label6.Text = "已讀取" + companys.Count + "間公司資訊";
            }
            else
            {
                if (type.Equals("top"))
                {
                    label6.Text = "讀取TOP中 " + e.ProgressPercentage.ToString() + " %";
                }
                else
                {
                    label6.Text = "計算中 " + e.ProgressPercentage.ToString() + " %";
                }
                
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            loadDataAndCompute();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                filePath = f.FileName;
                bw.RunWorkerAsync();
            }

        }
        private void loadDataAndCompute()
        {
            initValueFromTable();
            createCompanyTableFromFile();
            computeAndWriteData();
        }
        private void initValueFromTable()
        {
            fileName = textBox4.Text;
            string output = strPath + fileName;
            if (File.Exists(output))
            {
                File.Delete(output);
            }
            observation = Convert.ToInt32(textBox1.Text);
            skip = Convert.ToInt32(textBox2.Text);
            holding = Convert.ToInt32(textBox3.Text);
            top = Convert.ToInt32(textBox5.Text);
            winnerP = Convert.ToInt32(textBox6.Text);
        }
        private void createCompanyTableFromFile()
        {
            string line;
            companys.Clear();
            StreamReader sr = new StreamReader(filePath);
            while ((line = sr.ReadLine()) != null)
            {
                string[] h = line.Split(',');
                priceSize = h.Length;
                double[] tempDouble = new double[priceSize];
                int priceIndex = 0;
                foreach (string t in h)
                {
                    tempDouble[priceIndex] = Convert.ToDouble(t);
                    priceIndex++;
                }
                companys.Add(new Company(tempDouble, observation, holding, skip));
                bw.ReportProgress(0);
            }
            sr.Close();
            if (topFilePath.Equals("")) return;
            sr = new StreamReader(topFilePath);
            int index = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] h = line.Split(',');
                priceSize = h.Length;
                int[] tempInt = new int[priceSize];
                int priceIndex = 0;
                foreach (string t in h)
                {
                    tempInt[priceIndex] = Convert.ToInt32(t);
                    priceIndex++;
                }
                companys[index++].setTop(tempInt);
                Console.WriteLine(index);
                bw.ReportProgress((int)((double)index * 100.0 / (double)companys.Count()));
            }
            type = "";

        }
        private void computeAndWriteData()
        {
            string output = strPath + fileName;
            FileStream fs = File.Create(output);//new FileStream(strPath, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StringBuilder sb = new StringBuilder();
            int initIndex = 0;
            while (initIndex + observation + skip + holding < priceSize)
            {
                foreach (Company c in companys)
                {
                    c.reload(initIndex, observation, holding, skip);
                }
                List<Company> tempCompanys = new List<Company>();
                foreach (Company c in companys)
                {
                    if (!c.hasZero(observation + skip + 1))
                    {
                        tempCompanys.Add(c);
                    }
                }
                tempCompanys.Sort();
                int winner = (int)(tempCompanys.Count() * winnerP / 100);
                for (int j = 1; j <= winner; j++)
                {
                    Company c = tempCompanys[tempCompanys.Count() - j];
                    if (c.hasTop()) {
                        if (c.getTop(initIndex + observation + skip - 1) >= top)
                        {
                            sw.Write(c.getRate() + ",");
                        }
                    }
                    else
                    {
                        sw.Write(c.getRate() + ",");
                    }
                    
                }
                sw.Write("\n");
                initIndex++;
                bw.ReportProgress((int)((double)initIndex * 100.0/(double)priceSize));
            }
            sw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bw.RunWorkerAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                topFilePath = f.FileName;
            }
        }
    }
}
