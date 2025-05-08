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

            Load();
        }

        private async void Load()
        {
            烟尘记游戏 game = null;

            //随机显示李任语录
            Random rd = new();
            Rebillion_words_textblock.Text = "       " + Rebillion_qoute.rebillion_qoutes[rd.Next(0,Rebillion_qoute.rebillion_qoutes.Length)];

            //初始化游戏逻辑对象（利用所选择的存档内容)
            Task Task_load = Task.Run(() => game = new(Save.saves[Option.save_choose - 1]));

            Task Task_wait = Task.Run(() => Thread.Sleep(3000));       //等待四秒，(如果最终剧情不够加载四秒，就凑够四秒)

            await Task_load;

            await Task_wait;    //对应于上面的等待四秒

            Data.Main_window.Page_frame.NavigationService.Navigate(new Game(game));           //跳转到Game界面
        }
    }
}
