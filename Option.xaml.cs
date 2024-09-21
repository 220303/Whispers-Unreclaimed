using System.Text.RegularExpressions;

namespace 烟尘记
{
    /// <summary>
    /// Option.xaml 的交互逻辑
    /// </summary>
    public partial class Option : Page
    {
        public Option()
        {
            InitializeComponent();

            Plot_font_size_Textbox.Text = Convert.ToString(Data.Options.Plot_font_size);
            Plot_print_speed_Textbox.Text = Convert.ToString(Data.Options.Plot_print_speed);
            Music_volume_Textbox.Text = Convert.ToString(Data.Options.Music_volume);

            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))
            {
                NavigationService.GetNavigationService(this).GoBack();
            }
        }

        private void Save_options_Click(object sender, RoutedEventArgs e)
        {
            if(Regex.IsMatch(Plot_font_size_Textbox.Text, @"^\d+$") && Regex.IsMatch(Plot_print_speed_Textbox.Text, @"^\d+$") && Regex.IsMatch(Music_volume_Textbox.Text, @"^\d+$"))
            {
                Data.Options.Plot_font_size = ToInt32(Plot_font_size_Textbox.Text);
                Data.Options.Plot_print_speed = ToInt32(Plot_print_speed_Textbox.Text);
                Data.Options.Music_volume = ToInt32(Music_volume_Textbox.Text);
            }
            /*
            MessageBox.Show(Convert.ToString(Data.Options.Plot_font_size));
            MessageBox.Show(Convert.ToString(Data.Options.Plot_print_speed));
            MessageBox.Show(Convert.ToString(Data.Options.Music_volume));
            */
        }
    }
}
