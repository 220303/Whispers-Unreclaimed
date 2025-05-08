namespace 烟尘记
{
    /// <summary>
    /// Game.xaml 的交互逻辑
    /// </summary>
    public partial class Game : Page
    {
        private enum mode : int                     //状态指示变量 ： 0 为程序正在输出内容（此时可能在输出选项或剧情），此时不接受用户任何输入  1 为输出剧情   2 为输出选项   3 为将用户的输入视为选择选项
        {
            printing = 0,
            wait_plot_print = 1,
            wait_choices_print = 2,
            wait_choose = 3,
        }

        mode now_mode;

        烟尘记游戏 game;


        public Game(烟尘记游戏 game)
        {
            InitializeComponent();

            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上

            this.game = game;                                                                                    //接受从Rebillion页面传过来的game对象）

            now_mode = mode.wait_plot_print;                                                                     //标准初始化前台游戏参数（与存档内容无关）
        }

        private void finish_view()                                                                           //结局时执行的结尾操作
        {
            game.Save_game();                                                                                //保存游戏进度
            game = null;                                                                                     //将game对象放入垃圾处理器，等待.net平台回收
            Data.Main_window.Page_frame.NavigationService.Navigate(new Trace(false, false));                  //跳转到Trace页面
        }




        public void Plot_print()
        {
            //逐字输出剧情

            for (int i = 0; i < game.Nodes[game.Jump - 1].Plot.Length; i++)
            {


                // 使用 Dispatcher 在 UI 线程上更新 PlotText
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PlotText.Text += game.Nodes[game.Jump - 1].Plot[i];
                    Plot_scrollviewer.ScrollToBottom();
                });

                Thread.Sleep(Option.plot_print_speed);                        //这里是为了实验方便，成品后会去掉注释

            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                PlotText.Text += "\n";                                                      //输出完后换行，便于下一次输出。
            });

        }

        private void Choice_print()
        {
            if (game.Nodes[game.Jump - 1].Choices == null)             //如果没有选项，说明是结局，立即触发结局事件
            {
                finish_view();
            }
            else                                                       //如果有选项，则输出选项
            {

                // 清空之前的列定义
                Choices_grid.ColumnDefinitions.Clear();


                //筛选出能输出的选项(不能输出的直接删掉),在这之后game.Nodes[game.Jump - 1].Choices中只剩下能输出的选项
                for (int i =0; i< game.Nodes[game.Jump - 1].Choices.Count;i++)
                {
                    if (!game.Nodes[game.Jump - 1].Choices[i].Check(game.Record))
                    {
                        game.Nodes[game.Jump - 1].Choices.RemoveAt(i);
                        //由于删除了一个元素，后面的元素会向前移动，所以i要减一
                        i--;
                    }
                }


                //设置按钮左右留白的占比，按钮以及按钮间留空的占比(按钮:留空=6:4)
                double space_rate = 0.4;
                double button_space_rate = 1-space_rate;

                //计算 按钮的相对大小
                double button_space_ratio = button_space_rate / game.Nodes[game.Jump - 1].Choices.Count;

                //计算 左右侧留空，按钮间留空 的相对大小 (左右侧留空:按钮间留空=2:1)
                double space_unit_width = space_rate / (game.Nodes[game.Jump - 1].Choices.Count - 1 + 4);
                double left_right_space_ratio = 2 * space_unit_width;
                double between_buttons_space_ratio = space_unit_width;


                //创建左侧留空
                Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(left_right_space_ratio, GridUnitType.Star) });

                for(int i=0;i< game.Nodes[game.Jump - 1].Choices.Count;i++)
                {
                    //注意，i+1 是按钮的序数，比如i=0时代表第一个按钮

                    //创建按钮列
                    Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(button_space_ratio, GridUnitType.Star) });
                    
                    //创建按钮
                    Button choicebutton = new()
                    {
                        Name = "Choice" + Convert.ToString(i + 1),  //由于button的name属性必须以字母开头，所以不得不在前面加上Choice,后面处理时会去掉
                        Content = game.Nodes[game.Jump - 1].Choices[i].Content,
                        Visibility = Visibility.Visible
                    };

                    Grid.SetColumn(choicebutton, (i+1)*2-1);                         //将按钮填入为其创建的列中，注意(i+1)*2-1是因为i和列数从0开始，而button的序数从1开始
                    choicebutton.Click += new RoutedEventHandler(choose_click);      //给每一个choicebutton赋予一样的响应点击事件
                    Choices_grid.Children.Add(choicebutton);                         //将每一个choicebutton添加到choicesdock中

                    //创建按钮间留空(如果不是最后一个按钮的话)
                    if ((i+1) < game.Nodes[game.Jump - 1].Choices.Count)
                    {
                        Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(between_buttons_space_ratio, GridUnitType.Star) });
                    }
                }

                //创建右侧留空
                Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(left_right_space_ratio, GridUnitType.Star) });

                //输出选项到屏幕
                Choices_grid.Visibility = Visibility.Visible;                     //把dockpannal改成可见的（默认是隐藏的）

                // 强制更新布局
                Choices_grid.UpdateLayout();
            }
        }




        private async void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new Pause(game));          //跳转到Pause界面
            }
            else if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))                //输入 shift+; 进入Option界面
            {
                Data.Main_window.Page_frame.NavigationService.Navigate(new Options());          //跳转到Option界面
            }
            else if ((e.Key != Key.LeftShift) && (e.Key != Key.RightShift))                   //按其余任意键（不离开游戏）
            {
                switch (now_mode)
                {
                    case mode.wait_plot_print:                                                                                      //输出剧情

                        now_mode = mode.printing;

                        Task Task_Plot_print;
                        Task_Plot_print = Task.Run(() => Plot_print());
                        await Task_Plot_print;

                        now_mode = mode.wait_choices_print;

                        break;

                    case mode.wait_choices_print:                                                                                    //输出选项

                        now_mode = mode.printing;

                        Choice_print();

                        now_mode = mode.wait_choose;

                        break;

                    case mode.wait_choose:                                                                                           //确认用户选择

                        if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                        {

                            now_mode = mode.printing;

                            int choicenumber = ((int)e.Key + 6) % 10;              //把e.Key传递过来的枚举值（实际上是一个数字，代表了键盘上的某一个按键）转换为game.choose方法可以接受的值
                            game.Choose(choicenumber);

                            Choices_grid.Children.Clear();                                                   //把choicedock中的按钮清空
                            Choices_grid.Visibility = Visibility.Collapsed;                                  //把choicesdock改成隐藏的（不占用空间）


                            //输出下一段剧情
                            Task_Plot_print = Task.Run(() => Plot_print());
                            await Task_Plot_print;

                            now_mode = mode.wait_choices_print;
                        }

                        break;
                }
            }
        }

        private async void Page_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (now_mode)
            {
                case mode.wait_plot_print:                                                                 //输出剧情

                    now_mode = mode.printing;

                    Task Task_Plot_print = Task.Run(() => Plot_print());
                    await Task_Plot_print;

                    now_mode = mode.wait_choices_print;

                    break;

                case mode.wait_choices_print:

                    now_mode = mode.printing;

                    Choice_print();                                                                         //输出选项

                    now_mode = mode.wait_choose;

                    break;
            }
        }                       //响应用户的鼠标单击输入(mode == 1|2)


        private async void choose_click(object sender, RoutedEventArgs e)
        {
            //若用户点击选项按钮所执行的操作：将按钮的name经处理（去掉之前不得不加的choice）后（剩下的就是数字，可以直接传给input）传给input，执行choose函数

            Button btn = sender as Button;                                                  //获取用户点击的那个按钮
            int choose_number = Convert.ToInt16((btn.Name).Substring(6));                   //储存用该按钮名称计算出的选项编号
            game.Choose(choose_number);


            Choices_grid.Children.Clear();                                                   //把choicedock中的按钮清空
            Choices_grid.Visibility = Visibility.Collapsed;                                  //把choicesdock改成隐藏的（不占用空间）


            //输出下一段剧情
            now_mode = mode.printing;

            Task Task_Plot_print = Task.Run(() => Plot_print());
            await Task_Plot_print;

            now_mode = mode.wait_choices_print;

        }                       //响应用户的鼠标单击输入(mode == 3)









        //WPF选项输出思路：选项是一个grid视后台choices<List>的长度生成的一个好几列的行，
        //并且在其中每一个都动态生成填充满一个背景空白的按钮，内容直接连接到plotcut字段，选择完毕时直接把行高改成零，就算是没了。
        /*如：
         * Button btn = new Button
                 {
                     Name = "Button1",
                     Content = "OK",
                     Height = 23,
                     Width = 64,
                     HorizontalAlignment = HorizontalAlignment.Left,
                     Margin = new Thickness(10, 10, 0, 0),
                     VerticalAlignment = VerticalAlignment.Top,
                     Visibility = Visibility.Visible
        */

    }
}

