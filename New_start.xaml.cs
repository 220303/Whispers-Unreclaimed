namespace 烟尘记
{
    /// <summary>
    /// New_start.xaml 的交互逻辑
    /// </summary>
    public partial class New_start : Page
    {
        public New_start()
        {
            InitializeComponent();
        }


        private void New_save_Click(object sender, RoutedEventArgs e)
        {
            if (!(string.IsNullOrEmpty(New_save_name.Text)) && !(string.IsNullOrWhiteSpace(New_save_name.Text)))
            {
                Save.saves.Add
                (
                    new Save.save()
                    {
                        Name = New_save_name.Text,
                        Jump = 1,
                        Record = new List<string>() { "0-0" }
                    }
                );
                Option.save_choose = (Save.saves.Count);
                Option.Wrtie_out();
                Data.Main_window.Page_frame.NavigationService.Navigate(new Rebillion_qoutes());           //跳转到Rebillion界面
            }
        }



        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Data.Main_window.Page_frame.NavigationService.GoBack();
        }
    }
}
