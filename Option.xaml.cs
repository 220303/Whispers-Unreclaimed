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

            Plot_font_size_Slider.Value = Option.plot_font_size;
            Plot_print_speed_Slider.Value = Option.plot_print_speed;
            Music_volume_Slider.Value = Option.music_volume;

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

            Option.plot_font_size = (double)Plot_font_size_Slider.Value;

            Option.plot_print_speed = (Int16)(Plot_print_speed_Slider.Value);

            Option.music_volume = Music_volume_Slider.Value;

            //使得音量更改立即生效
            Music_player.music_player.Volume = Option.music_volume;



            //保存Options到文件
            Option.Wrtie_out();

            //恢复按钮响应
            ((Button)sender).IsEnabled = true;

        }

        #endregion

    }

    public class F0FontSizeToStringConverter : IValueConverter
    //滑块数值显示转换器
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("F0"); // 显示整数部分
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out double result))
                return result;
            return 0;
        }
    }
    public class F2FontSizeToStringConverter : IValueConverter
    //滑块数值显示转换器
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("F2"); // 显示2位小数
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out double result))
                return result;
            return 0;
        }
    }
    public class Music_FontSizeToStringConverter : IValueConverter
    //滑块数值显示转换器
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Math.Round((double)value,3)*1000).ToString(); //将slider的value四舍五入到三位小数再乘1000变为整数，再转换为字符串显示。
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out double result))
                return result;
            return 0;
        }
    }
}
