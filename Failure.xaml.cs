using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace 烟尘记
{
    /// <summary>
    /// Failure.xaml 的交互逻辑
    /// </summary>
    public partial class Failure : Page
    {
        public Failure()
        {
            InitializeComponent();

            Restart_text.Text = "后悔是徒然的...\n也许故事并不令人后悔。\n但总有一种可能，不至于如此......\n";
            Restart_text.Text += "(游戏无法读档，只能重新开始)\n(也许线索就留在过去的某个瞬间)\n(请再接再厉...)\n";
            Restart_text.Text += "祝好运！";
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                //F12键在MainWindow里面处理，这里不处理
                return;
            }

            end();
        }


        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
          end();
        }

        private void end()
        {
            Option.save_choose = 0;                                                //重置存档选择
            Page_frame.Navigate(new Start());                                      //点击鼠标进入Start页面
        }

    }
}
