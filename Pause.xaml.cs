using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Pause.xaml 的交互逻辑
    /// </summary>
    public partial class Pause : Page
    {
        public Pause(烟尘记游戏 game)
        {
            InitializeComponent();
            this.game = game;
            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                NavigationService.GetNavigationService(this).GoBack();
            }
        }



        烟尘记游戏 game;
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            game.Dispose();                                                               //调用game的Dispose方法，实际上是保存游戏进度
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            //调用关闭代码
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.Close();
        }

        private void Quit_to_Start_Click(object sender, RoutedEventArgs e)
        {
            game = null;
            NavigationService.GetNavigationService(this).Navigate(new Start());           //跳转到Start界面
        }
    }
}
