namespace 烟尘记
{
    /// <summary>
    /// Start.xaml 的交互逻辑
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }


        private void start_game()
        {
            //开始游戏会有两种情况，一是什么都存档都没有，则进入New_start界面创建新的存档；二是有存档，那么Options文件中必然有save_choose值，则进入Ribillion界面。
            if (Option.save_choose == 0)
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new New_start());           //跳转到New_start界面
            }
            else
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new Rebillion_qoutes());           //跳转到Rebillion界面
            }
        }


        private MainWindow main_window = Application.Current.MainWindow as MainWindow;
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {

            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))                      //输入 shift+; 进入Option界面
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new Options());                                 //跳转到Option界面
            }
            else if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.A))                            //输入 shift+a 进入Trace 界面
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new Trace(true, true));                        //跳转到Trace界面
            }
            else if ((e.Key != Key.LeftShift) && (e.Key != Key.RightShift))                                                //按其余任意键进入游戏
            {
                start_game();
            }
        }


        private void Page_MouseDown(object sender, MouseButtonEventArgs e)                             //点击屏幕任意区域进入游戏
        {
            start_game();
        }
    }
}

