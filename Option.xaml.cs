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


        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))
            {
                Data.Main_window.Page_frame.NavigationService.GoBack();
            }
        }

        private void Save_options_Click(object sender, RoutedEventArgs e)
        {
            if(Regex.IsMatch(Plot_font_size_Textbox.Text, @"^\d+$") && Regex.IsMatch(Plot_print_speed_Textbox.Text, @"^\d+$") && Regex.IsMatch(Music_volume_Textbox.Text, @"^\d+$"))
            {
                Option.plot_font_size = ToInt32(Plot_font_size_Textbox.Text);
                Option.plot_print_speed = ToInt32(Plot_print_speed_Textbox.Text);
                Option.music_volume = ToDouble(Music_volume_Textbox.Text) / 10000.0;

                //使得音量更改立即生效
                Music_player.music_player.Volume = Option.music_volume;

                //保存Options到文件
                Option.Wrtie_out();
            }
        }
    }
}
