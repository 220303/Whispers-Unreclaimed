namespace 烟尘记
{
    /// <summary>
    /// Trace.xaml 的交互逻辑
    /// </summary>
    public partial class Trace : Page
    {
        public Trace(bool operation, bool back) //接受数字参数： 可/不可 操作   可/不可 退回
        {
            InitializeComponent();
            this.operation = operation;
            this.back = back;
            Saves_ListBox.ItemsSource = Data.Saves;

            if(Data.Option.Save_choose != 0)                                                             //刚安装好游戏时默认是0，此时不高亮SelectItem(显蓝)
                Saves_ListBox.SelectedItem = Data.Saves[Data.Option.Save_choose - 1];                    //通过高亮SelectItem(显蓝)显示选中的存档

            if (operation ==false)
            {
                New_save.Visibility = Visibility.Collapsed;
                Delete_save.Visibility = Visibility.Collapsed;
            }

            Saves_ListBox.Background = Brushes.AliceBlue;
            this.Loaded += (s, e) => this.Focus();                                                        //确保焦点汇聚到page上
        }


        private bool operation;

        private bool back;




        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (back)
            {
                if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.A))
                {
                    Data.Main_window.Page_frame.NavigationService.GoBack();
                }
            }
            else
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new Start());          //跳转到Start界面
            }
        }




        private void Saves_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (operation)
            {
                if (Saves_ListBox.SelectedItem != null)
                {
                    Data.Save selected = (Data.Save)Saves_ListBox.SelectedItem;
                    Data.Option.Save_choose = Data.Saves.IndexOf(selected)+1;
                    Data.Option_wrtie_out();
                }
            }
        }



        private void Delete_save_Click(object sender, RoutedEventArgs e)
        {
            if (operation)
            {
                if (Saves_ListBox.SelectedItem != null)
                {
                    Data.Save selected = (Data.Save)Saves_ListBox.SelectedItem;
                    if (Data.Option.Save_choose == Data.Saves.IndexOf(selected))
                    {
                        Data.Option.Save_choose =0;
                        //Data.Options_wrtie_out();
                    }
                    Data.Saves.Remove(selected);
                    File.Delete(Data.Saves_directory_path + selected.Name + ".txt");
                }
            }
        }

        private void New_save_Click(object sender, RoutedEventArgs e)
        {
            if (operation)
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new New_start());           //跳转到New_start界面
            }
        }
    }
}