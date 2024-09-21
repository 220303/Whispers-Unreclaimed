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

            //Start start = new Start();                                //实例化PageSelect，初始选择页ps
            //FrameWork.Content = new Frame() { Content = start };        //mainwindow中建立frame，用来承载所有的page，用PageSelect作为初始页面

            FrameWork.Content = new Frame() { Content = new Start() };   //mainwindow中建立frame，用来承载所有的page，用Start作为初始页面
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {

            // 退出时把存档List写入存档文件
            foreach (Data.Save save in Data.Saves)
            {
                Data.Save_write_out();
            }

            // 退出时把Options写入设置文件
            Data.Options_wrtie_out();
        }
    }
}
