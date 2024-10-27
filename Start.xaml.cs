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

            //加载存档到Data.Saves(List)中
            Data.Save_read_in();


            //加载 Options 文件到 Data中
            Data.Options_read_in();

            //加载 Rebillion Qoutes文件到 Rebillion_qoutes（string[]）中 
            Data.Rebillion_qoutes_read_in();

        }

        private void start_game()
        {
            if (Data.Options.Save_choose == 0)
            {
                NavigationService.GetNavigationService(this).Navigate(new New_start());           //跳转到New_start界面
            }
            else
            {
                NavigationService.GetNavigationService(this).Navigate(new Rebillion_qoutes());           //跳转到Rebillion界面
            }
        }



        private void Page_KeyDown(object sender, KeyEventArgs e)
        {

            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))                      //输入 shift+; 进入Option界面
            {
                NavigationService.GetNavigationService(this).Navigate(new Option());                                //跳转到Option界面
            }
            else if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.A))                            //输入 shift+a 进入Trace 界面
            {
                NavigationService.GetNavigationService(this).Navigate(new Trace(true, true));                        //跳转到Trace界面
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

