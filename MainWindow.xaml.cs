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
                Data.Global_music_start("program");
            }, DispatcherPriority.Background);



            //——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————



            //在Mainwindow中建立frame，用来承载所有的page，用Start作为初始页面
            Page_frame.Content = new Frame() { Content = new Start() };


            //如果是刚刚进入游戏(Saves.Count == 0)，则加载存档到Data.Saves(List)中
            if (Data.Saves.Count == 0)            
                Data.Save_read_in();

            //如果是刚刚进入游戏，则加载Options.txt到Data.Options中
            if (EqualityComparer<Data.Option_struct>.Default.Equals(Data.Option, default(Data.Option_struct)))            
                Data.Option_read_in();

            //如果是刚刚进入游戏，则加载 Rebillion Qoutes文件到 Rebillion_qoutes（string[]）中 
            if (Data.Rebillion_qoutes == null)            
                Data.Rebillion_qoutes_read_in();

        }
    }
}
