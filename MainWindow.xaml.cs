using System.Windows.Media.Animation;

namespace 烟尘记
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            //初始化全局Main_window对象
            Data.Main_window = this;

            //必须等待页面初始化完成后才能初始化音乐播放器
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                //初始化全局音乐播放器
                Music_player.Start("program");
            }, DispatcherPriority.Background);



            //——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————




            //加载存档到Data.Saves(List)中
            Save.Read_in();

            //加载Options.txt到Data.Options中
            Option.Read_in();

            //加载 Rebillion Qoutes文件到 Rebillion_qoutes（string[]）中 
            Rebillion_qoute.Read_in();

            //在中Page_frame中用Start作为初始页面
            Page_frame.Content = new Frame() { Content = new Start() };

            // 监听导航事件
            Page_frame.Navigating += MainFrame_Navigating;
            Page_frame.Navigated += MainFrame_Navigated;

        }





        // 页面切出事件（导航开始前）
        private void MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

            BlackMask.Visibility = Visibility.Visible;

            // 取消默认导航（先执行动画）
            e.Cancel = true;

            //避免重复触发事件
            Page_frame.Navigating -= MainFrame_Navigating;

            // 获取动画资源
            var storyboard = ((Storyboard)BlackMask.Resources["Page_out"]).Clone();

            // 动画完成时跳转
            EventHandler handler = null;
            handler = (s, args) =>
            {
                // 防止事件累积
                storyboard.Completed -= handler;

                // 执行实际导航
                Page_frame.Navigate(e.Content);

                //恢复事件监听
                Page_frame.Navigating += MainFrame_Navigating;
            };

            //为克隆体添加事件监听
            storyboard.Completed += handler;
            // 启动动画
            storyboard.Begin(BlackMask);
        }



        // 页面切入事件（导航完成后）
        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

            //避免重复触发事件
            Page_frame.Navigated -= MainFrame_Navigated;

            // 获取动画资源
            var storyboard = ((Storyboard)BlackMask.Resources["Page_in"]).Clone();

            // 动画完成时跳转
            EventHandler handler = null;
            handler = (s, args) =>
            {
                // 防止事件累积
                storyboard.Completed -= handler;

                // 隐藏蒙版
                BlackMask.Visibility = Visibility.Collapsed;

                //恢复事件监听
                Page_frame.Navigated += MainFrame_Navigated;
            };

            //为克隆体添加事件监听
            storyboard.Completed += handler;

            // 启动动画
            storyboard.Begin(BlackMask);
        }

    }
}
