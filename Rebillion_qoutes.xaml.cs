using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 烟尘记
{
    /// <summary>
    /// Rebillion_qoutes.xaml 的交互逻辑
    /// </summary>
    public partial class Rebillion_qoutes : Page
    {
        public Rebillion_qoutes()
        {
            InitializeComponent();

            wait();
        }

        private async void wait()
        {

            Task Task_load = Task.Run(() => Load());

            Task Task_wait = Task.Run(() => Thread.Sleep(3000));       //等待四秒

            await Task_load;

            await Task_wait;


            //随机显示李任语录
            //Random rd = new();
            //Rebillion_words_textblock.Text = "       " + Data.Rebillion_qoutes[rd.Next(0, Data.Rebillion_qoutes.Length)];

            NavigationService.GetNavigationService(this).Navigate(new Game());           //跳转到Game界面
        }

        private void Load()
        {
            if (Data.Nodes.Count == 0)
                Data.Nodes_read_in();
        }
    }
}
