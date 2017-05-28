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
        string filePath = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ColumnHeader header1 = new ColumnHeader();
            header1.Text = "";
            header1.TextAlign = HorizontalAlignment.Left;
            header1.Width = 700;
            listView1.Columns.Add(header1);
            listView1.View = View.Details;

           

        }

        
        

        public class Company : IComparable<Company>
        {
            double compare = 0;
            double rateOfReturn = -1;
            double[] companyPrice;
            int initIndex = 0;
            public Company(double[] companyPrice, int observation, int holding, int skip)
            {
                this.companyPrice = companyPrice;
                computeCompareValue(this.companyPrice, observation);
                computeRateOfReturn(this.companyPrice, observation, holding, skip);
            }
            public void reload(int initIndex,int observation, int holding, int skip)
            {
                this.initIndex = initIndex;
                computeCompareValue(this.companyPrice, observation);
                computeRateOfReturn(this.companyPrice, observation, holding, skip);
            }


            int IComparable<Company>.CompareTo(Company other)
            {
                return compare - other.compare > 0 ? 1 : compare - other.compare == 0 ? 0 : -1;
            }

            private void computeCompareValue(double[] companyPrice, int observation)
            {
                double max = 0;
                int size = Math.Min(observation+ initIndex, companyPrice.Length);
                for (int i = initIndex; i < size; i++)
                {
                    if (companyPrice[i] > max)
                    {
                        max = companyPrice[i];
                    }
                }
                compare = companyPrice[initIndex+observation - 1] / max;
            }
            private void computeRateOfReturn(double[] companyPrice, int observation, int holding, int skip)
            {
                int first = Math.Min((initIndex + observation + skip -1), companyPrice.Length);
                int last = Math.Min((initIndex + observation + skip + holding-1), companyPrice.Length);
                rateOfReturn = (companyPrice[last] - companyPrice[first]) / companyPrice[first];

            }

            public string getString()
            {
                return "compare :" + compare + ", rate : " + rateOfReturn;
            }
            public bool hasZero(int observation)
            {
                int size = Math.Min(companyPrice.Length, observation + initIndex);
                for (int i = initIndex; i < size; i++)
                {
                    if (companyPrice[i] == 0)
                    {
                        return true;
                    }
                }
                return false;
            }


        }


        private void button1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog f = new OpenFileDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                filePath = f.FileName;
                loadDataAndCompute();
            }

        }
        private void loadDataAndCompute()
        {
            observation = Convert.ToInt32(textBox1.Text);
            skip = Convert.ToInt32(textBox2.Text);
            holding = Convert.ToInt32(textBox3.Text);
            listView1.Clear();
            ColumnHeader header1 = new ColumnHeader();
            header1.Text = "內容";
            header1.TextAlign = HorizontalAlignment.Left;
            header1.Width = 700;
            listView1.Columns.Add(header1);
            listView1.View = View.Details;
            StringBuilder sb = new StringBuilder();
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
            }
            
            int initIndex = 0;
            while (initIndex + observation + skip + holding < priceSize)
            {
                ListViewItem i = new ListViewItem();
                i.Text = "-------"+ initIndex + "---------";
                listView1.Items.Add(i);
                foreach (Company c in companys)
                {
                    c.reload(initIndex, observation, holding, skip);
                }
                List<Company> tempCompanys = new List<Company>();
                foreach (Company c in companys)
                {
                    if (!c.hasZero(observation))
                    {
                        tempCompanys.Add(c);
                    }
                }
                tempCompanys.Sort();
                foreach (Company c in tempCompanys)
                {
                    ListViewItem ii = new ListViewItem();
                    ii.Text = c.getString();
                    listView1.Items.Add(ii);
                }
                initIndex++;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadDataAndCompute();
        }
    }
}
