using System.Net.NetworkInformation;
using System.Printing.IndexedProperties;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
namespace 烟尘记
{

    /// <summary>
    /// Option.xaml 的交互逻辑
    /// </summary>
    public partial class Options : Page
    {
        public Options()
        {
            InitializeComponent();

            Plot_font_size_Textbox.Text = Convert.ToString(Option.plot_font_size);
            Plot_print_speed_Textbox.Text = Convert.ToString(Option.plot_print_speed);
            Music_volume_Textbox.Text = Convert.ToString(Option.music_volume * 10000);

            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }


        public static Button_animation.ParticleColorScheme Green_colors => new Button_animation.ParticleColorScheme
        {
            Gradient_start = Color.FromRgb(149, 213, 178), // #95D5B2
            Gradient_end = Color.FromArgb(0, 149, 213, 178), //透明
            GlowColor = Color.FromRgb(82, 183, 136)     // #52B788
        };


        #region 页面交互逻辑
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))
            {
                Page_frame.Go_back();
            }
        }


        private async void Save_options_Click(object sender, RoutedEventArgs e)
        {

            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Green_colors);
            if (Regex.IsMatch(Plot_font_size_Textbox.Text, @"^\d+$"))
            {
                Option.plot_font_size = ToInt32(Plot_font_size_Textbox.Text);
            }
            if (Regex.IsMatch(Plot_print_speed_Textbox.Text, @"^\d+$"))
            {
                Option.plot_print_speed = ToInt32(Plot_print_speed_Textbox.Text);
            }
            if (Regex.IsMatch(Music_volume_Textbox.Text, @"^\d+$"))
            {

                Option.music_volume = ToDouble(Music_volume_Textbox.Text) / 10000.0;

                //使得音量更改立即生效
                Music_player.music_player.Volume = Option.music_volume;

            }

            //保存Options到文件
            Option.Wrtie_out();

            //恢复按钮响应
            ((Button)sender).IsEnabled = true;

        }

        #endregion

    }

}
