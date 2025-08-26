using System.Windows.Media.Animation;
using static 烟尘记.Save;

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
            Rebillion_words_textblock.Text = "   " + Rebillion_qoute.rebillion_qoutes[rd.Next(0,Rebillion_qoute.rebillion_qoutes.Length)];

            //根据save_choose的值来选择存档(其值代表活动存档的UUID)，然后初始化游戏逻辑对象（利用所选择的存档内容)
            Task Task_load = Task.Run(() => game = new(Save.saves.FirstOrDefault(s => s.UUID == Option.save_choose)));
            Task Task_wait = Task.Run(() => Thread.Sleep(4500));       //等待六秒(由于入场动画原因，用户看到语录的事件只有不到四秒)(如果最终剧情不够加载六秒，就凑够六秒)

            await Task_load;

            await Task_wait;    //对应于上面的等待六秒

            Page_frame.Navigate(new Game(game));           //跳转到Game界面
        }
    }
}
