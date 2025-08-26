namespace 烟尘记
{
    /// <summary>
    /// Credit.xaml 的交互逻辑
    /// </summary>
    public partial class Credit : Page
    {

        //Credit页面相关路径
        private static string Credit_path = Data.Filesystem_directory_path + "/Credit/";
        private static string plot_trailer_path = Credit_path + "Plot_trailer.txt";
        private static string my_word_path = Credit_path + "My_word.txt";
        private static string Rebillion_word_path = Credit_path + "Rebillion_word.txt";
        private static string credit_text_path = Credit_path + "Credit.txt";
        private static string finish_path = Credit_path + "Finish.txt";

        private bool credit_end;


        public Credit()
        {

            credit_end = false; //初始化credit_end为false，表示尚未结束

            InitializeComponent();

            //读取文本文件内容到对应的文本框
            finish.Text = File.ReadAllText(finish_path);
            plot_trailer.Text = File.ReadAllText(plot_trailer_path);
            my_word.Text = File.ReadAllText(my_word_path);
            Rebillion_word.Text = File.ReadAllText(Rebillion_word_path);
            credit_text.Text = File.ReadAllText(credit_text_path);

            //设置占位文本框的内容
            blank_0.Text = "\n\n\n\n\n\n\n\n\n"; 
            blank_1.Text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n";
            blank_2.Text = "\n\n\n";
            blank_3.Text = "\n\n\n\n\n";
            blank_word.Text = "\n\n\n";
            blank_4.Text = "\n\n\n\n\n\n\n\n";

            //我和李任的话提示词
            me.Inlines.Add(new Run("220303") { Foreground = Brushes.Cyan });
            me.Inlines.Add(new Run(" >>> ") { Foreground = Brushes.DarkOrange });
            rebillion.Inlines.Add(new Run(" <<< ") { Foreground = Brushes.ForestGreen });
            rebillion.Inlines.Add(new Run("Rebillion") { Foreground = Brushes.Magenta });


            //确保焦点汇聚到page上
            this.Loaded += (s, e) => this.Focus();                                                         
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        //致谢滚动
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                double viewerHeight = ((ScrollViewer)scrolling_panel.Parent).ActualHeight;
                double contentHeight = scrolling_panel.ActualHeight;

                // 从下面移进来，到上面移出去
                var animation = new DoubleAnimation
                {
                    From = viewerHeight,
                    To = -contentHeight,
                    Duration = TimeSpan.FromSeconds(370),  //总滚动时间/秒
                    RepeatBehavior = new RepeatBehavior(1) // 循环次数(Forever 无限循环)
                };

                // 注册动画完成后的回调
                animation.Completed += (s, args) =>
                {
                    // ✅ 滚动完成后执行的代码
                    credit_end = true;                  // 设置credit_end为true，表示滚动结束
                };

                scrolling_translate.BeginAnimation(TranslateTransform.YProperty, animation);
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }


        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                //F12键在MainWindow里面处理，这里不处理
                return;
            }
            if (credit_end)
            {
                end();
            }
            if (Option.credit_completed)
            {
                if(e.Key == Key.Escape)
                {
                    end();
                }
            }
        }


        private void Page_MouseDown(object sender, MouseButtonEventArgs e) 
        {
            if (credit_end)
            {
                end();
            }
        }

        private void end()
        {
            Option.credit_completed = true;                                     //标记为已经看过致谢
            Option.save_choose = 0;                                                //重置存档选择
            Page_frame.Navigate(new Start());                   //如果滚动结束了，点击鼠标进入Start页面
        }
    }
}
