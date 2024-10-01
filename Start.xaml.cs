using Microsoft.Win32;

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

        private async void start_game()
        {
            /*
            //更换背景，显示随机李任语录，停留 4 秒钟。
            Random rd = new();
            Rebillion_words_textblock.Text = "       " + Data.Rebillion_qoutes[rd.Next(0, Data.Rebillion_qoutes.Length)];
            Rebillion_words_textblock.Visibility = Visibility.Visible;
            Quotes_lable.Visibility = Visibility.Visible;
            Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("Resource/image/Rebillion Quotes.png", UriKind.RelativeOrAbsolute)), Stretch = Stretch.Uniform };

            this.IsEnabled = false;                // 禁用输入
            await Task.Delay(3000);                // 等待4秒
            this.IsEnabled = true;                 // 恢复输入
            */
            NavigationService.GetNavigationService(this).Navigate(new Game());           //跳转到Game界面
        }



        private void Page_KeyDown(object sender, KeyEventArgs e)                              
        {

            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))                      //输入 shift+; 进入Option界面
            {
                NavigationService.GetNavigationService(this).Navigate(new Option());          //跳转到Option界面
            }
            else if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.A))                            //输入 shift+a 进入Trace 界面
            {
                NavigationService.GetNavigationService(this).Navigate(new Trace(true,true));           //跳转到Trace界面
            }
            else if((e.Key != Key.LeftShift) && (e.Key != Key.RightShift))                                                //按其余任意键进入游戏
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

