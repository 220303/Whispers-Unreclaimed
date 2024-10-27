using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace 烟尘记
{
    /// <summary>
    /// Trace.xaml 的交互逻辑
    /// </summary>
    public partial class Trace : Page
    {
        public Trace(bool operation, bool back) //接受数字参数： 可/不可 操作   可/不可 退回
        {
            InitializeComponent();
            this.operation = operation;
            this.back = back;
            Saves_ListBox.ItemsSource = Data.Saves;
            Saves_ListBox.SelectedItem = Data.Saves[Data.Options.Save_choose-1];                                   //显示正在哪个存档中
            Saves_ListBox.Background = Brushes.AliceBlue;
            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }


        private bool operation;

        private bool back;




        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (back)
            {
                if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.A))
                {
                    NavigationService.GetNavigationService(this).GoBack();
                }
            }
            else
            {
                NavigationService.GetNavigationService(this).Navigate(new Start());          //跳转到Start界面
            }
        }




        private void Saves_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (operation)
            {
                if (Saves_ListBox.SelectedItem != null)
                {
                    Data.Save selected = (Data.Save)Saves_ListBox.SelectedItem;
                    Data.Options.Save_choose = Data.Saves.IndexOf(selected)+1;
                }
            }
        }



        private void Delete_save_Click(object sender, RoutedEventArgs e)
        {
            if (operation)
            {
                if (Saves_ListBox.SelectedItem != null)
                {
                    Data.Save selected = (Data.Save)Saves_ListBox.SelectedItem;
                    if (Data.Options.Save_choose == Data.Saves.IndexOf(selected))
                    {
                        Data.Options.Save_choose -= 1;
                    }
                    Data.Saves.Remove(selected);
                    File.Delete(Data.Saves_directory_path + selected.Name + ".txt");
                }
            }
        }

        private void New_save_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GetNavigationService(this).Navigate(new New_start());           //跳转到New_start界面
        }
    }
}