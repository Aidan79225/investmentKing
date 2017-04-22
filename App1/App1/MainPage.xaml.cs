using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x404

namespace App1
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        double[][] companyPrice;
        List<Company> companys = new List<Company>();
        int observation = 12;
        int holding = 12;
        int skip = 2;
        int companysSize = 20000;
        int priceSize = 4000;
        public MainPage(){
            this.InitializeComponent();
            init();
            companys.Sort();
            foreach (Company c in companys) {
                ListViewItem i = new ListViewItem();
                i.Content = c.getString();
                listView.Items.Add(i);

            }
            


        }

        private void init(){
            companyPrice = new double[companysSize][ ];
            for (int i = 0; i < companyPrice.Length;i++){
                companyPrice[i] = new double[priceSize];
                Random r = new Random(i);
                for (int j = 0; j < priceSize; j++) {
                    companyPrice[i][j] = r.NextDouble() * 10;
                }
            }
            for (int i = 0; i < companyPrice.Length;i++) {
                if (hasZero(companyPrice[i],observation)) {
                    continue;
                }
                else{
                    companys.Add(new Company(companyPrice[i], observation,holding,skip));
                }
            }
        }
        private bool hasZero(double[] companyPrice,int observation){
            int size = Math.Min(companyPrice.Length, observation);
            for (int i = 0 ; i < size ; i++ ){
                if (companyPrice[i] == 0){
                    return true;
                }
            }
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e){
            
        }

    }

    public class Company : IComparable<Company>
    {
        double compare = 0;
        double rateOfReturn = -1;
        public Company(double[] companyPrice, int observation,int holding,int skip){
            computeCompareValue(companyPrice, observation);
            computeRateOfReturn(companyPrice, observation, holding, skip);
        }

        int IComparable<Company>.CompareTo(Company other){
            return compare - other.compare > 0 ? 1 : compare - other.compare == 0 ? 0 : -1;
        }

        private void computeCompareValue(double[] companyPrice, int observation){
            double max = 0;
            int size = Math.Min(observation, companyPrice.Length);
            for (int i = 0; i < size ; i++){
                if(companyPrice[i] > max){
                    max = companyPrice[i];
                }
            }
            compare = companyPrice[observation - 1] / max;
        }
        private void computeRateOfReturn(double[] companyPrice, int observation, int holding, int skip) {
            double first = Math.Min(observation + skip + 1, companyPrice.Length);
            double last = Math.Min(observation + skip + holding , companyPrice.Length);
            rateOfReturn = (last - first) / first;
        }

        public string getString()
        {
            return "compare :" + compare + ", rate : " + rateOfReturn;
        }


    }
}
