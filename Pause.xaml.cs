namespace 烟尘记
{
    /// <summary>
    /// Pause.xaml 的交互逻辑
    /// </summary>
    public partial class Pause : Page
    {

        烟尘记游戏 game;
        public Pause(烟尘记游戏 game)
        {
            InitializeComponent();
            this.game = game;
            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }


        public static Button_animation.ParticleColorScheme Blue_colors => new Button_animation.ParticleColorScheme
        {
            Gradient_start = Color.FromRgb(163, 245, 255),           // #A3F5FF 浅青蓝
            Gradient_end = Color.FromArgb(0, 163, 245, 255),
            GlowColor = Color.FromRgb(0, 225, 245)                   // #00E1F5 高亮青蓝
        };



        #region 页面交互逻辑
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Page_frame.Go_back();
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {

            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Blue_colors);

            game.Save_game();                                                               //保存游戏进度

            //恢复按钮响应
            ((Button)sender).IsEnabled = true;

        }

        private async void Quit_Click(object sender, RoutedEventArgs e)
        {

            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Blue_colors);

            //关闭整个程序
            Data.Main_window.Close();

            //恢复按钮响应
            ((Button)sender).IsEnabled = true;

        }

        private async void Quit_to_Start_Click(object sender, RoutedEventArgs e)
        {

            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Blue_colors);

            game = null;
            Page_frame.Navigate(new Start());           //跳转到Start界面

            //恢复按钮响应
            ((Button)sender).IsEnabled = true;
        }

        #endregion

    }
}


/*
 *        原按钮布局
 *             <Grid.ColumnDefinitions>
                <ColumnDefinition Width="0.8*" />
                <ColumnDefinition Width="0.8*" />
                <ColumnDefinition Width="1*" />
                <ColumnDefinition Width="0.8*" />
                <ColumnDefinition Width="1*" />
            </Grid.ColumnDefinitions>

            <Grid.RowDefinitions>
                <RowDefinition Height="3*" />
                <RowDefinition Height="1.5*" />
                <RowDefinition Height="1*" />
                <RowDefinition Height="1.5*" />
                <RowDefinition Height="1*" />
                <RowDefinition Height="1.5*" />
                <RowDefinition Height="3*" />
            </Grid.RowDefinitions>
*/