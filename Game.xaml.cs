namespace 烟尘记
{
    /// <summary>
    /// Game.xaml 的交互逻辑
    /// </summary>
    public partial class Game : Page
    {
        public static Button_animation.ParticleColorScheme Gold_colors => new Button_animation.ParticleColorScheme
        {
            Gradient_start = Color.FromRgb(255, 215, 0),              // #FFD700
            Gradient_end = Color.FromArgb(0, 255, 215, 0),            // 透明金色
            GlowColor = Color.FromRgb(255, 215, 0)                    // #FFD700
        };

        烟尘记游戏 game;

        //状态指示变量 ： 0 为程序正在输出内容（此时可能在输出选项或剧情），此时不接受用户任何输入  1 为输出剧情   2 为输出选项   3 为将用户的输入视为选择选项
        private enum mode : int
        {
            printing = 0,
            wait_plot_print = 1,
            wait_choices_print = 2,
            wait_choose = 3,
        }

        mode now_mode;
        public Game(烟尘记游戏 game)
        {
            InitializeComponent();

            this.game = game;                                                                                    //接受从Rebillion页面传过来的game对象）


            //注册加载星星事件和清除星星事件ArgumentException: “40”不是属性“FontSize”的有效值。
            Loaded += (s, e) => InitializeSpaceEffects();
            Unloaded += (s, e) => Cleanup();

            this.Save_name.Text = game.Name;

            now_mode = mode.wait_plot_print;                                                                     //标准初始化前台游戏参数（与存档内容无关）

            this.Loaded += (s, e) => this.Focus();                                                               //确保加载完后焦点汇聚到Game页面上

        }



        #region 用户输入
        private async void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.now_mode != mode.printing)
            {
                if (e.Key == Key.F12)
                {
                    //F12键在MainWindow里面处理，这里不处理
                }
                else if (e.Key == Key.Escape)
                {
                    Page_frame.Navigate(new Pause(game));          //跳转到Pause界面
                }
                else if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))                //输入 shift+; 进入Option界面
                {
                    Page_frame.Navigate(new Options());          //跳转到Option界面
                }
                else if ((e.Key != Key.LeftShift) && (e.Key != Key.RightShift))                   //按其余任意键（不离开游戏）
                {
                    switch (now_mode)
                    {
                        case mode.wait_plot_print:                                                                                      //输出剧情

                            now_mode = mode.printing;

                            await Text_print_verbatim(game.Nodes[game.Jump - 1].Plot);

                            now_mode = mode.wait_choices_print;

                            break;

                        case mode.wait_choices_print:                                                                                    //输出选项

                            now_mode = mode.printing;

                            await Choice_print();

                            now_mode = mode.wait_choose;

                            break;

                        case mode.wait_choose:                                                                                           //确认用户选择

                            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                            {
                                now_mode = mode.printing;

                                if ((((int)e.Key + 6) % 10) <= Choices_grid.Children.Count)
                                {
                                    //获取用户选择的按钮，((int)e.Key + 6) % 10是因为:e.Key传递过来的枚举值（实际上是一个数字，代表了键盘上的某一个按键）,需要转换为按钮的编号。
                                    var choosen_button = (Button)Choices_grid.Children[(((int)e.Key + 6) % 10) - 1];
                                    //获取用户点击的那个按钮并执行选择函数(包括动画等逻辑)
                                    await Choose_button(choosen_button);
                                }

                                now_mode = mode.wait_choices_print;
                            }

                            break;
                    }
                }
            }
        }

        private async void Page_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsHitTestVisible = false;

            switch (now_mode)
            {
                case mode.wait_plot_print:                                                                 //输出剧情

                    now_mode = mode.printing;

                    await Text_print_verbatim(game.Nodes[game.Jump - 1].Plot);

                    now_mode = mode.wait_choices_print;

                    break;

                case mode.wait_choices_print:

                    now_mode = mode.printing;

                    await Choice_print();                                                                         //输出选项

                    now_mode = mode.wait_choose;

                    break;
            }

            this.IsHitTestVisible = true;
        }


        private async void choose_click(object sender, RoutedEventArgs e)
        //响应用户的鼠标单击输入(mode == 3)
        {
            //锁键盘、鼠标(必须锁DockPanal，否则用户可能以极快的手速选好几个选项...)，改游戏状态
            Choices_grid.IsHitTestVisible = false;
            now_mode = mode.printing;

            //获取用户点击的那个按钮并执行选择函数(包括动画等逻辑)
            await Choose_button((Button)sender);

            //开键盘鼠标，改状态
            now_mode = mode.wait_choices_print;
            Choices_grid.IsHitTestVisible = true;
        }

        #endregion



        #region 页面响应逻辑

        public async Task Text_print_verbatim(List<烟尘记游戏.Text_fragment> fragments)
        {
            foreach (var fragment in fragments)
            {
                // 拆分文本为字符数组并逐字输出（保留换行）
                foreach (char ch in fragment.Text)
                {
                    // 使用 Dispatcher 在 UI 线程上更新 PlotText
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (ch == '\n')
                        {
                            PlotText.Inlines.Add(new LineBreak());
                        }
                        else
                        {
                            // 在 UI 线程中创建并配置 Run
                            var run = new Run(ch.ToString())
                            {
                                FontFamily = fragment.Style.FontFamily,
                                FontSize = fragment.Style.FontSize,
                                Foreground = fragment.Style.Foreground,
                                FontWeight = fragment.Style.Bold ? FontWeights.Bold : FontWeights.Normal,
                                FontStyle = fragment.Style.Italic ? FontStyles.Italic : FontStyles.Normal,
                                TextDecorations = new TextDecorationCollection()
                            };

                            if (fragment.Style.Underline)
                                run.TextDecorations.Add(TextDecorations.Underline);
                            if (fragment.Style.Strikethrough)
                                run.TextDecorations.Add(TextDecorations.Strikethrough);

                            PlotText.Inlines.Add(run);
                            Plot_scrollviewer.ScrollToBottom();
                        }
                    });

                    await Task.Delay(Option.plot_print_speed);
                }

            }


            //空一行，并且输出完后换行，便于下一次输出。
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                PlotText.Inlines.Add(new LineBreak());
                PlotText.Inlines.Add(new LineBreak());
            });

        }

        private async Task Text_print_verbatim(string text)
        {

            //将text利用default_text_style转换为fragment
            var fragment = new 烟尘记游戏.Text_fragment
            {
                Style = game.default_text_style,
                Text = text,
            };


            // 将fragment拆分文本为字符数组并逐字输出
            foreach (char ch in fragment.Text)
            {

                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (ch == '\n')
                    {
                        PlotText.Inlines.Add(new LineBreak());
                    }
                    else
                    {
                        // 在 UI 线程中创建并配置 Run
                        var run = new Run(ch.ToString())
                        {
                            FontFamily = fragment.Style.FontFamily,
                            FontSize = fragment.Style.FontSize,
                            Foreground = fragment.Style.Foreground,
                            FontWeight = fragment.Style.Bold ? FontWeights.Bold : FontWeights.Normal,
                            FontStyle = fragment.Style.Italic ? FontStyles.Italic : FontStyles.Normal,
                            TextDecorations = new TextDecorationCollection()
                        };

                        if (fragment.Style.Underline)
                            run.TextDecorations.Add(TextDecorations.Underline);
                        if (fragment.Style.Strikethrough)
                            run.TextDecorations.Add(TextDecorations.Strikethrough);

                        PlotText.Inlines.Add(run);
                        Plot_scrollviewer.ScrollToBottom();
                    }
                });

                await Task.Delay(Option.plot_print_speed);
            }


            //空一行，并且输出完后换行，便于下一次输出。
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                PlotText.Inlines.Add(new LineBreak());
                PlotText.Inlines.Add(new LineBreak());
            });
        }

        private async Task Choice_print()
        {
            if (game == null)
            //如果用户狂按键盘让事件堆积到finish_view都把game变成null了，那这个也没必要处理了。
            {
                return;
            }
            else if (game.Nodes[game.Jump - 1].Choices == null)             //如果没有选项，说明是结局，立即触发结局事件
            {
                finish_view();
            }
            else                                                       //如果有选项，则输出选项
            {

                #region 更新选项布局

                // 清空之前的列定义
                Choices_grid.ColumnDefinitions.Clear();

                //筛选出能输出的选项(不能输出的直接删掉),在这之后game.Nodes[game.Jump - 1].Choices中只剩下能输出的选项
                for (int i = 0; i < game.Nodes[game.Jump - 1].Choices.Count; i++)
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
                double button_space_rate = 1 - space_rate;

                //计算 按钮的相对大小
                double button_space_ratio = button_space_rate / game.Nodes[game.Jump - 1].Choices.Count;

                //计算 左右侧留空，按钮间留空 的相对大小 (左右侧留空:按钮间留空=2:1)
                double space_unit_width = space_rate / (game.Nodes[game.Jump - 1].Choices.Count - 1 + 4);
                double left_right_space_ratio = 2 * space_unit_width;
                double between_buttons_space_ratio = space_unit_width;


                //创建左侧留空
                Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(left_right_space_ratio, GridUnitType.Star) });

                for (int i = 0; i < game.Nodes[game.Jump - 1].Choices.Count; i++)
                {
                    //注意，i+1 是按钮的序数，比如i=0时代表第一个按钮

                    //创建按钮列
                    Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(button_space_ratio, GridUnitType.Star) });

                    //创建按钮
                    Button choicebutton = new()
                    {
                        Name = "Choice" + Convert.ToString(i + 1),  //由于button的name属性必须以字母开头，所以不得不在前面加上Choice,后面处理时会去掉
                        Content = game.Nodes[game.Jump - 1].Choices[i].Content,
                        Visibility = Visibility.Visible,
                    };

                    Grid.SetColumn(choicebutton, (i + 1) * 2 - 1);                   //将按钮填入为其创建的列中，注意(i+1)*2-1是因为i和列数从0开始，而button的序数从1开始
                    choicebutton.Click += new RoutedEventHandler(choose_click);      //给每一个choicebutton赋予一样的响应点击事件
                    Choices_grid.Children.Add(choicebutton);                         //将每一个choicebutton添加到choicesdock中

                    //创建按钮间留空(如果不是最后一个按钮的话)
                    if ((i + 1) < game.Nodes[game.Jump - 1].Choices.Count)
                    {
                        Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(between_buttons_space_ratio, GridUnitType.Star) });
                    }
                }

                //创建右侧留空
                Choices_grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(left_right_space_ratio, GridUnitType.Star) });

                #endregion

                #region 输出选项到屏幕
                //把dockpannal改成可见的
                Choices_grid.Visibility = Visibility.Visible;
                //播放Choices_grid淡入动画
                Task AwaitStoryboardAsync(Storyboard storyboard, DependencyObject target)
                {
                    var tcs = new TaskCompletionSource<bool>();
                    EventHandler handler = null;
                    handler = (s, e) =>
                    {
                        //事件解绑，下次还会绑定
                        storyboard.Completed -= handler;
                        tcs.SetResult(true);
                    };
                    //绑定事件
                    storyboard.Completed += handler;
                    Storyboard.SetTarget(storyboard, target);
                    storyboard.Begin();
                    return tcs.Task;
                }
                // 等待动画完成
                await AwaitStoryboardAsync((Storyboard)this.FindResource("Choices_grid_fade_in"), Choices_grid);

                // 强制更新布局
                Choices_grid.UpdateLayout();

                #endregion
            }
        }

        private async Task Choose_button(Button choosen_button)
        {
            //播放粒子效果并耐心等待播放完毕
            await Button_animation.Animation(choosen_button, Gold_colors); //播放按钮动画

            //输出选择的选项的内容
            await Text_print_verbatim((string)choosen_button.Content);

            //若用户点击选项按钮所执行的操作：将按钮的name经处理（去掉之前不得不加的choice）后（剩下的就是数字，可以直接传给input）传给input，执行choose函数
            int choose_number = Convert.ToInt16((choosen_button.Name).Substring(6));                   //储存用该按钮名称计算出的选项编号
            game.Choose(choose_number);

            //封装Choices_grid淡出动画
            Task AwaitStoryboardAsync(Storyboard storyboard, DependencyObject target)
            {
                var tcs = new TaskCompletionSource<bool>();
                EventHandler handler = null;
                handler = (s, e) =>
                {
                    //事件解绑，下次还会绑定
                    storyboard.Completed -= handler;
                    tcs.SetResult(true);
                };
                //绑定事件
                storyboard.Completed += handler;
                Storyboard.SetTarget(storyboard, target);
                storyboard.Begin();
                return tcs.Task;
            }

            //播放并等待动画完成
            await AwaitStoryboardAsync((Storyboard)this.FindResource("Choices_grid_fade_out"), Choices_grid);


            //更新Choices页面显示状态
            Choices_grid.Children.Clear();                                                   //把choicedock中的按钮清空
            Choices_grid.Visibility = Visibility.Collapsed;                                  //把choicesdock改成隐藏的（不占用空间）


            //输出下一段剧情
            await Text_print_verbatim(game.Nodes[game.Jump - 1].Plot);

            // 把焦点还给页面本身
            Keyboard.Focus(this);
        }

        private void finish_view()                                                                           //结局时执行的结尾操作
        {
            game.Save_game();                                                                                //保存游戏进度
            game = null;                                                                                     //将game对象放入垃圾处理器，等待.net平台回收
            Page_frame.Navigate(new Credit());                            //跳转到Credit页面
        }

        #endregion




        #region 恒星闪烁
        private const int MaxStars = 100;
        private const int MaxStarSize = 5;
        private const double StarSpawnRate = 0.6;

        private void InitializeStarSystem()
        {
            CompositionTarget.Rendering += OnStarFrameUpdate;
        }

        private void OnStarFrameUpdate(object sender, EventArgs e)
        {
            if (Star_field.ActualWidth == 0 || Star_field.ActualHeight == 0) return;

            var time = ((RenderingEventArgs)e).RenderingTime;
            if (_random.NextDouble() < StarSpawnRate * time.TotalSeconds / 10)
            {
                CreateNewStar();
            }
        }

        private void CreateNewStar()
        {
            if (Star_field.Children.Count >= MaxStars) return;

            var star = new Ellipse
            {
                Width = _random.Next(1, MaxStarSize + 1),
                Height = _random.Next(1, MaxStarSize + 1),
                Fill = Brushes.White,
                Opacity = 0
            };

            Canvas.SetLeft(star, _random.NextDouble() * Star_field.ActualWidth);
            Canvas.SetTop(star, _random.NextDouble() * Star_field.ActualHeight);

            Star_field.Children.Add(star);
            StartStarLifecycle(star);
        }

        private void StartStarLifecycle(Ellipse star)
        {
            var blinkAnim = new DoubleAnimation
            {
                From = 0,
                To = _random.NextDouble() * 0.5 + 0.5,
                Duration = TimeSpan.FromSeconds(_random.NextDouble() * 1.5 + 0.5), // 1.5~2.0秒
                AutoReverse = true,
                EasingFunction = new SineEase()
            };

            blinkAnim.Completed += (s, e) => SafeRemoveElement(star);
            star.BeginAnimation(UIElement.OpacityProperty, blinkAnim);
        }
        #endregion



        #region 流星效果
        private DispatcherTimer _meteorTimer;
        private const double MeteorSpawnChance = 0.6;
        private const int MinMeteorSpeed = 200;
        private const int MaxMeteorSpeed = 300;
        private const int MinMeteorLength = 180;
        private const int MaxMeteorLength = 250;


        /// <summary>
        /// 初始化流星系统
        /// </summary>
        private void InitializeMeteorSystem()
        {
            _meteorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3),
                IsEnabled = true
            };
            _meteorTimer.Tick += (s, e) => TryCreateMeteor();
        }


        /// <summary>
        /// 获取屏幕外边缘点（带偏移）
        /// </summary>
        private Point GetEdgePointWithOffset(int edge)
        {
            const int offset = 100; // 屏幕外偏移量
            return edge switch
            {
                0 => new Point(-offset, _random.NextDouble() * Star_field.ActualHeight),           // 左
                1 => new Point(Star_field.ActualWidth + offset, _random.NextDouble() * Star_field.ActualHeight), // 右
                2 => new Point(_random.NextDouble() * Star_field.ActualWidth, -offset),            // 上
                _ => new Point(_random.NextDouble() * Star_field.ActualWidth, Star_field.ActualHeight + offset) // 下
            };
        }

        /// <summary>
        /// 创建正确方向的流星
        /// </summary>
        private void CreateMeteor(Point start, Point end, int speed, int size, Color color)
        {
            // 计算运动向量
            Vector direction = end - start;
            if (direction.Length < 10) return; // 距离太近直接丢弃，避免数值不稳定
            direction.Normalize();

            // 彗尾长度（根据速度动态计算）
            double length = _random.Next(MinMeteorLength, MaxMeteorLength);

            // 彗尾方向（运动反方向）
            Point tailPoint = new Point(-direction.X * length, -direction.Y * length);

            // 创建路径（头部在原点，尾部指向反方向）
            var path = new PathGeometry();
            path.Figures.Add(new PathFigure
            {
                StartPoint = new Point(0, 0),
                Segments = { new LineSegment(tailPoint, true) }
            });

            // 渐变刷（头部实色，尾部透明）
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0.5),
                EndPoint = new Point(1, 0.5)
            };
            brush.GradientStops.Add(new GradientStop(color, 0.0)); // 头部
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1.0)); // 尾部

            // 旋转角度 = 运动方向（无需额外补偿）
            double angle = Math.Atan2(direction.Y, direction.X) * 180 / Math.PI + 180;

            var meteor = new System.Windows.Shapes.Path
            {
                StrokeThickness = size,
                Stroke = brush,
                Data = path,
                RenderTransform = new TransformGroup
                {
                    Children =
                    {
                        new TranslateTransform(start.X, start.Y),
                        new RotateTransform(angle)
                    }
                },
                Opacity = 0
            };

            Star_field.Children.Add(meteor);
            StartMeteorAnimation(meteor, start, end, speed, length);
        }

        /// <summary>
        /// 启动流星动画
        /// </summary>
        private void StartMeteorAnimation(System.Windows.Shapes.Path meteor, Point start, Point end, int speed, double length)
        {
            var tg = meteor.RenderTransform as TransformGroup;
            var tt = tg.Children[0] as TranslateTransform;

            // 计算总距离（包含屏幕外区域）
            double totalDistance = (end - start).Length + length * 2;
            double duration = totalDistance / speed;

            // 移动动画
            var moveAnimX = new DoubleAnimation(end.X, TimeSpan.FromSeconds(duration));
            var moveAnimY = new DoubleAnimation(end.Y, TimeSpan.FromSeconds(duration));

            // 淡入淡出动画
            var fadeInOut = new DoubleAnimationUsingKeyFrames();
            fadeInOut.KeyFrames.Add(new LinearDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            fadeInOut.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration * 0.2))));
            fadeInOut.KeyFrames.Add(new LinearDoubleKeyFrame(0.8, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration * 0.9))));
            fadeInOut.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration))));

            // 彗尾闪烁动画
            var tailBlink = new DoubleAnimation
            {
                From = 1,
                To = 0.7,
                Duration = TimeSpan.FromSeconds(0.15),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            meteor.Stroke.BeginAnimation(Brush.OpacityProperty, tailBlink);

            // 动画完成处理
            fadeInOut.Completed += (s, e) => SafeRemoveElement(meteor);

            tt.BeginAnimation(TranslateTransform.XProperty, moveAnimX);
            tt.BeginAnimation(TranslateTransform.YProperty, moveAnimY);
            meteor.BeginAnimation(UIElement.OpacityProperty, fadeInOut);
        }

        /// <summary>
        /// 尝试生成流星（修改后的版本）
        /// </summary>
        private void TryCreateMeteor()
        {
            if (_random.NextDouble() > MeteorSpawnChance) return;

            // 随机选择进入和退出边（不能相同）
            int entryEdge = _random.Next(4);
            int exitEdge = (entryEdge + _random.Next(1, 4)) % 4;

            Point start = GetEdgePointWithOffset(entryEdge);
            Point end = GetEdgePointWithOffset(exitEdge);

            // 确保流星横跨屏幕
            if (entryEdge == 0 && exitEdge == 1) end.X += 100; // 左→右
            if (entryEdge == 1 && exitEdge == 0) start.X -= 100; // 右→左
            if (entryEdge == 2 && exitEdge == 3) end.Y += 100; // 上→下
            if (entryEdge == 3 && exitEdge == 2) start.Y -= 100; // 下→上

            CreateMeteor(
                start: start,
                end: end,
                speed: _random.Next(MinMeteorSpeed, MaxMeteorSpeed),
                size: _random.Next(3, 6),
                color: Color.FromRgb(
                    (byte)_random.Next(200, 255),
                    (byte)_random.Next(200, 255),
                    (byte)_random.Next(220, 255))
            );
        }
        #endregion



        #region 公共方法
        private readonly Random _random = new Random();

        private void InitializeSpaceEffects()
        {
            InitializeStarSystem();
            InitializeMeteorSystem();
        }

        private void Cleanup()
        {
            CompositionTarget.Rendering -= OnStarFrameUpdate;
            _meteorTimer?.Stop();
            Star_field.Children.Clear();
        }

        private void SafeRemoveElement(UIElement element)
        {
            if (Star_field.Children.Contains(element))
                Star_field.Children.Remove(element);
        }

        #endregion



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

