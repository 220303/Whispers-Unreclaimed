namespace 烟尘记
{
    /// <summary>
    /// Rebillion_qoutes.xaml 的交互逻辑
    /// </summary>
    public partial class Rebillion_qoutes : Page
    {
        public Rebillion_qoutes()
        {
            InitializeComponent();

            wait();
        }

        private async void wait()
        {

            //随机显示李任语录
            Random rd = new();
            Rebillion_words_textblock.Text = "       " + Data.Rebillion_qoutes[rd.Next(0,Data.Rebillion_qoutes.Length)];

            //加载所有剧情和选项
            Task Task_load = Task.Run(() => Load());

            Task Task_wait = Task.Run(() => Thread.Sleep(3000));       //等待四秒，(如果最终剧情不够加载四秒，就凑够四秒)

            await Task_load;

            await Task_wait;    //对应于上面的等待四秒

            Data.Main_window.Page_frame.NavigationService.Navigate(new Game());           //跳转到Game界面
        }

        private void Load()
        {
            //如果是游戏启动后第一次开始一局游戏(此时没加载过Nodes，则Data.Nodes.Count就是0)，则读取所有剧情和选项，如果之前已经玩过一局或多局游戏，则不需要读取
            if (Data.Nodes.Count == 0)
                Data.Nodes_read_in();
        }
    }
}
