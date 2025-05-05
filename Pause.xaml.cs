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
                Data.Main_window.Page_frame.NavigationService.GoBack();
            }
        }



        烟尘记游戏 game;
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            game.Save_game();                                                               //保存游戏进度
            Data.Save_write_out();                                                          //保存Saves到文件
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            //调用关闭代码
            Data.Main_window.Close();
        }

        private void Quit_to_Start_Click(object sender, RoutedEventArgs e)
        {
            game = null;
            Data.Main_window.Page_frame.NavigationService.Navigate(new Start());           //跳转到Start界面
        }
    }
}
