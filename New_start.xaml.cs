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
    /// New_start.xaml 的交互逻辑
    /// </summary>
    public partial class New_start : Page
    {
        public New_start()
        {
            InitializeComponent();
        }
        private void New_save_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrEmpty(New_save_name.Text)) && !(string.IsNullOrWhiteSpace(New_save_name.Text)))
            {
                Data.Saves.Add
                (
                    new Data.Save()
                    {
                        Name = New_save_name.Text,
                        Jump = 1,
                        Record = new List<string>() { "0-0" }
                    }
                );
                Data.Options.Save_choose = (Data.Saves.Count - 1);
                NavigationService.GetNavigationService(this).Navigate(new Rebillion_qoutes());           //跳转到Rebillion界面
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this).GoBack();
        }
    }
}
