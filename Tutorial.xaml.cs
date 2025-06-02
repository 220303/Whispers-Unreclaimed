namespace 烟尘记
{
    /// <summary>
    /// Tutorial.xaml 的交互逻辑
    /// </summary>
    public partial class Tutorial : Page
    {

        //教程内容
        private string tutorial_text;

        //防抖标志值
        private bool input_permit;


        public Tutorial()
        {
            InitializeComponent();

            tutorial_text = File.ReadAllText(Data.Filesystem_directory_path + "Tutorial.txt");                   //从教程文件中读取所有内容

            input_permit = true;                                                                                 //允许输入

            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }

        private void Start_game()
        {
            //开始游戏会有两种情况，一是什么都存档都没有，则进入New_start界面创建新的存档；二是有存档，那么Option文件中必然有save_choose值，则进入Ribillion_qoutes页面。
            if (Option.save_choose == 0)
            {
                Page_frame.Navigate(new New_start());                                           //跳转到New_start界面
            }
            else
            {
                Page_frame.Navigate(new Rebillion_qoutes());                                    //跳转到Rebillion界面
            }
        }


        private async Task Text_print_verbatim(string text)
        //逐字输出函数
        {
            for (int i = 0; i < text.Length; i++)
            {

                // 使用 Dispatcher 在 UI 线程上更新 Tutorial_text
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Tutorial_text.Text += text[i];
                    Tutorial_scrollviewer.ScrollToBottom();
                });
                await Task.Delay(Option.plot_print_speed);                                  //这里是为了实验方便，成品后会去掉注释

            }
        }

        private async void Page_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (input_permit)
            {
                input_permit = false;                                                               //禁止输入,防抖
                if (Option.tutorial_completed == 1)
                {
                    Start_game();                                                            //开始游戏
                }

                if (Option.tutorial_completed == 0)
                {
                    await Text_print_verbatim(tutorial_text);                                       //逐字打印教程内容
                    Option.tutorial_completed = 1;
                    Option.Wrtie_out();                                                             //写入配置文件
                }
                input_permit = true;                                                                //允许输入
            }
        }

        private async void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (input_permit)
            {
                input_permit = false;                                                               //禁止输入,防抖
                if (e.Key == Key.F12)
                {
                    //F12键在MainWindow里面处理，这里不处理
                }
                else if (Option.tutorial_completed == 1)
                {
                    Start_game();                                                            //开始游戏
                }
                else if (Option.tutorial_completed == 0)
                {
                    await Text_print_verbatim(tutorial_text);                                    //逐字打印教程内容
                    Option.tutorial_completed = 1;
                    Option.Wrtie_out();                                                             //写入配置文件
                }
                input_permit = true;                                                              //允许输入
            }
        }
    }
}
