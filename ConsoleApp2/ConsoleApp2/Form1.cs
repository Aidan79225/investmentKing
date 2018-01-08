using compute;
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
        List<RegressionX> regressionXs = new List<RegressionX>();
        int observation = 12;
        int holding = 1;
        int skip = 0;
        int priceSize = 4696;
        int top = 0;
        int winnerP = 30;
        string filePath = "";
        string sizeFilePath = "";
        string strPath = @"c:\temp\";
        string fileName = "MyTest.csv";
        const string COMPARE = "compare-";
        const string REGRESSION = "regression-";
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
            string output2 = strPath + COMPARE + fileName;
            if (File.Exists(output2))
            {
                File.Delete(output2);
            }
            string output3 = strPath + REGRESSION + fileName;
            if (File.Exists(output3))
            {
                File.Delete(output3);
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
            int index = 0;
            if (sizeFilePath.Length > 0)
            {
                sr = new StreamReader(sizeFilePath);
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
                    companys[index].setCompanySize(tempDouble);
                    index++;
                    bw.ReportProgress(0);
                }
                sr.Close();
            }




            index = 0;
            foreach (Company c in companys)
            {
                c.computeTop();
                bw.ReportProgress((int)((double)index * 100.0 / (double)companys.Count()));
            }
            type = "";

        }
        private void computeAndWriteData()
        {
            string output = strPath + fileName;
            string output2 = strPath + COMPARE + fileName;
            string output3 = strPath + REGRESSION + fileName;
            FileStream fs = File.Create(output);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StringBuilder sb = new StringBuilder();

            FileStream fs2 = File.Create(output2);
            StreamWriter sw2 = new StreamWriter(fs2, System.Text.Encoding.Default);
            StringBuilder sb2 = new StringBuilder();

            FileStream fs3 = File.Create(output3);
            StreamWriter sw3 = new StreamWriter(fs3, System.Text.Encoding.Default);
            StringBuilder sb3 = new StringBuilder();
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
                    if (c.getTop(initIndex + observation - 1) >= top)
                    {
                        sw.Write(c.getRate() + ",");
                        sw2.Write(c.getCompare() + ",");
                        regressionXs.Add(new RegressionX(c.getRemuneration(), c.companySize[initIndex + observation + skip - 1], 1, 1, c.getRate()));
                    }
                    else
                    {
                        regressionXs.Add(new RegressionX(c.getRemuneration(), c.companySize[initIndex + observation + skip - 1], 1, 0, c.getRate()));
                    }

                }
                for (int j = winner + 1; j <= tempCompanys.Count(); j++)
                {
                    Company c = tempCompanys[tempCompanys.Count() - j];
                    regressionXs.Add(new RegressionX(c.getRemuneration(), c.companySize[initIndex + observation + skip - 1], 0, 0, c.getRate()));
                }
                try
                {
                    double[] beta = computeRegression();
                    for (int i = 0; i < RegressionX.Size; i++)
                    {
                        sw3.Write(beta[i] + ",");
                    }
                    sw3.Write("\n");
                }
                catch (Exception e) { }
                regressionXs.Clear();
                sw.Write("\n");
                sw2.Write("\n");
                initIndex++;
                bw.ReportProgress((int)((double)initIndex * 100.0 / (double)priceSize));
            }
            sw.Close();
            sw2.Close();
            sw3.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bw.RunWorkerAsync();
        }

        private void printAllRegressions()
        {
            for (int i = 0; i < regressionXs.Count(); i++)
            {
                Console.Write("[" + i + "]:");
                for (int j = 0; j < RegressionX.Size; j++)
                {
                    Console.Write(regressionXs[i].x[j] + ",");
                }
                Console.Write("y:" + regressionXs[i].y);
                Console.WriteLine("");
            }
            Console.WriteLine("RegressionX : " + regressionXs.Count());
        }

        private void printMatrix(double[][] matrix)
        {
            Console.Write("matrix:\n");
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    Console.Write(matrix[i][j] + ",");
                }
                Console.Write("\n");
            }
        }

        private double[] computeRegression()
        {
            double[][] temp;
            temp = new double[RegressionX.Size][];
            for (int i = 0; i < RegressionX.Size; i++)
            {
                temp[i] = new double[RegressionX.Size];
                for (int j = 0; j < RegressionX.Size; j++)
                {
                    double ans = 0.0;
                    for (int k = 0; k < regressionXs.Count(); k++)
                    {
                        ans += regressionXs[k].x[i] * regressionXs[k].x[j];
                    }
                    temp[i][j] = ans;
                }
            }
            double[][] inverse = InverseMatrix(temp);
            double[][] multiple;
            multiple = new double[RegressionX.Size][];
            for (int i = 0; i < RegressionX.Size; i++)
            {
                multiple[i] = new double[regressionXs.Count()];
                for (int j = 0; j < regressionXs.Count(); j++)
                {
                    double ans = 0.0;
                    for (int k = 0; k < RegressionX.Size; k++)
                    {
                        ans += inverse[i][k] * regressionXs[j].x[k];
                    }
                    multiple[i][j] = ans;
                }
            }


            double[] beta = new double[RegressionX.Size];
            for (int i = 0; i < RegressionX.Size; i++)
            {
                double ans = 0.0;
                for (int j = 0; j < regressionXs.Count(); j++)
                {
                    ans += multiple[i][j] * regressionXs[j].y;
                }
                beta[i] = ans;
            }
            return beta;



        }



        public static double[][] InverseMatrix(double[][] matrix)
        {
            //matrix必須为非空
            if (matrix == null || matrix.Length == 0)
            {
                return new double[][] { };
            }

            //matrix 必須为方陣
            int len = matrix.Length;
            for (int counter = 0; counter < matrix.Length; counter++)
            {
                if (matrix[counter].Length != len)
                {
                    throw new Exception("matrix 必須为方陣");
                }
            }


            //計算矩陣行列式的值
            double dDeterminant = Determinant(matrix);
            if (Math.Abs(dDeterminant) <= 1E-6)
            {
                throw new Exception("矩陣不可逆");
            }

            //制作一個伴隨矩陣大小的矩陣
            double[][] result = AdjointMatrix(matrix);

            //矩陣的每項除以矩陣行列式的值，即为所求
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    result[i][j] = result[i][j] / dDeterminant;
                }
            }

            return result;
        }

        public static double Determinant(double[][] matrix)
        {
            //二階及以下行列式直接計算
            if (matrix.Length == 0) return 0;
            else if (matrix.Length == 1) return matrix[0][0];
            else if (matrix.Length == 2)
            {
                return matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];
            }

            //對第一行使用“加邊法”遞歸計算行列式的值
            double dSum = 0, dSign = 1;
            for (int i = 0; i < matrix.Length; i++)
            {
                double[][] matrixTemp = new double[matrix.Length - 1][];
                for (int count = 0; count < matrix.Length - 1; count++)
                {
                    matrixTemp[count] = new double[matrix.Length - 1];
                }

                for (int j = 0; j < matrixTemp.Length; j++)
                {
                    for (int k = 0; k < matrixTemp.Length; k++)
                    {
                        matrixTemp[j][k] = matrix[j + 1][k >= i ? k + 1 : k];
                    }
                }

                dSum += (matrix[0][i] * dSign * Determinant(matrixTemp));
                dSign = dSign * -1;
            }

            return dSum;
        }

        public static double[][] AdjointMatrix(double[][] matrix)
        {
            //制作一個伴隨矩陣大小的矩陣
            double[][] result = new double[matrix.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[matrix[i].Length];
            }

            //生成伴隨矩陣
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result.Length; j++)
                {
                    double[][] temp = new double[result.Length - 1][];
                    for (int k = 0; k < result.Length - 1; k++)
                    {
                        temp[k] = new double[result[k].Length - 1];
                    }
                    for (int x = 0; x < temp.Length; x++)
                    {
                        for (int y = 0; y < temp.Length; y++)
                        {
                            temp[x][y] = matrix[x < i ? x : x + 1][y < j ? y : y + 1];
                        }
                    }
                    result[j][i] = ((i + j) % 2 == 0 ? 1 : -1) * Determinant(temp);
                }
            }
            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                sizeFilePath = f.FileName;
            }
        }
    }
}
