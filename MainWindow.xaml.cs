using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace 烟尘记
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private double left = 400;
        private double top = 300;
        private double width = 800;
        private double height = 450;




        public MainWindow()
        {
            InitializeComponent();

            #region 数据加载

            //加载存档到Data.Saves(List)中
            Save.Read_in();

            //加载Options.txt到Data.Options中
            Option.Read_in();

            //加载 Rebillion Qoutes文件到 Rebillion_qoutes（string[]）中 
            Rebillion_qoute.Read_in();

            #endregion

            #region UI、交互相关初始化

            //初始化全局Main_window对象
            Data.Main_window = this;

            //在中page_frame中用Start作为初始页面
            page_frame.Content = new Frame() { Content = new Start() };

            //初始化导航类
            Page_frame.Navigation_animation_Initialize(page_frame, black_mask);


            //必须等待页面初始化完成后才能初始化音乐播放器
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                //初始化全局音乐播放器
                Music_player.Start("program");
            }, DispatcherPriority.Background);

            #endregion

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)                                                                    //按F12键进入或退出全屏
            {
                if (this.WindowStyle == WindowStyle.SingleBorderWindow)
                {

                    // 恢复窗口尺寸为正常
                    Data.Main_window.WindowState = WindowState.Normal;

                    //记录窗口位置
                    left = this.Left;
                    top = this.Top;
                    width = this.Width;
                    height = this.Height;


                    //确保窗口置顶
                    this.Topmost = true;

                    // 设置标题栏隐藏
                    this.WindowStyle = WindowStyle.None;

                    // 设置窗口尺寸为全屏(包含任务栏)
                    this.WindowState = WindowState.Maximized;

                    // 禁用调整窗口大小
                    this.ResizeMode = ResizeMode.NoResize;

                }
                else if (this.WindowStyle == WindowStyle.None)
                {

                    // 设置标题栏显示
                    Data.Main_window.WindowStyle = WindowStyle.SingleBorderWindow;

                    // 恢复窗口尺寸为正常
                    Data.Main_window.WindowState = WindowState.Normal;

                    // 恢复调整模式
                    this.ResizeMode = ResizeMode.CanResize;

                    //取消强制置顶
                    this.Topmost = false;


                    // 恢复窗口位置和大小
                    this.Left = left;
                    this.Top = top;
                    this.Width = width;
                    this.Height = height;

                }
            }
        }
    }
}
