namespace 烟尘记
{
    /// <summary>
    /// Start.xaml 的交互逻辑
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();


            //从Option静态类中读取图片的移动速度
            this.speed = Option.start_picture_speed;
            //设置窗口大小改变时的调用UpdateDimensions()
            SizeChanged += (s, e) => Image_update_dimensions();
            //设置加载页面后自动初始化并开始动画
            InitializeAnimation();


            //初始化并开始水波纹动画
            Water_ripple_start();
            //设置窗口大小改变时的调用UpdateDimensions()
            SizeChanged += (s, e) => Watter_ripple_update_dimensions();

            this.Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }




        #region 水波纹部分

        //水波纹需要的定时器
        private DispatcherTimer rippleTimer = new DispatcherTimer();
        private DateTime startTime = DateTime.Now;

        private void Water_ripple_start()
        {
            // 启动水波动画
            rippleTimer.Interval = TimeSpan.FromMilliseconds(16);
            rippleTimer.Tick += (s, e) =>
            {
                float t = (float)(DateTime.Now - startTime).TotalSeconds;
                RippleEffect_1.Time = t;
                RippleEffect_2.Time = t;

            };
            rippleTimer.Start();

            // 鼠标移动时更新水波中心
            MouseMove += (s, e) =>
            {
                //获取鼠标在图片一上的位置，并设置水波纹中心
                var pos_1 = e.GetPosition(Image1);

                RippleEffect_1.Center = new Point(
                    pos_1.X / Image1.ActualWidth,
                    pos_1.Y / Image1.ActualHeight
                );

                //获取鼠标在图片二上的位置，并设置水波纹中心
                var pos_2 = e.GetPosition(Image2);

                RippleEffect_2.Center = new Point(
                    pos_2.X / Image2.ActualWidth,
                    pos_2.Y / Image2.ActualHeight
                );

            };

            // 鼠标进入时初始化中心
            MouseEnter += (s, e) =>
            {
                //获取鼠标在图片一上的位置，并设置水波纹中心
                var pos_1 = e.GetPosition(Image1);

                RippleEffect_1.Center = new Point(
                    pos_1.X / Image1.ActualWidth,
                    pos_1.Y / Image1.ActualHeight
                );

                //获取鼠标在图片二上的位置，并设置水波纹中心
                var pos_2 = e.GetPosition(Image2);

                RippleEffect_2.Center = new Point(
                    pos_2.X / Image2.ActualWidth,
                    pos_2.Y / Image2.ActualHeight
                );

            };
        }

        private void Watter_ripple_update_dimensions()
        {
            //设置水波shader的宽高比
            RippleEffect_1.AspectRatio = (float)(Image1.Width / Image1.Height);
            RippleEffect_2.AspectRatio = (float)(Image2.Width / Image2.Height);
        }

        #endregion



        #region 背景部分

        //图片的宽度(根据窗口高度动态改变)
        private double scaled_width;

        //移动速度，单位: 像素/秒
        private int speed;

        //图片偏移量(表示移动到什么位置了),始终是负值(因为是左移)
        private double offset;

        //上次移动图片的时间，下次移动时要用
        private DateTime last_update;


        private void InitializeAnimation()
        //此函数初始化动画
        {
            //设置初始“移动时间”为当前时间
            last_update = DateTime.Now;

            //每一帧都调用UpdatePosition()函数来更新图片位置，从而实现动画
            CompositionTarget.Rendering += UpdatePosition;

            //根据窗口大小设置图片的初始大小和位置
            Image_update_dimensions();
        }

        private void Image_update_dimensions()
        //此函数处理改变窗口大小后图片的大小及位置变化
        {
            //如果当前窗口高度小于等于0，则不进行任何操作
            if (ActualHeight <= 0) return;

            // 计算图片新的宽度(按照 48:9 的原始比例)
            double newWidth = ActualHeight * (48.0 / 9.0);

            // 按比例比例调整偏移量，以保持动画连续性
            if (scaled_width > 0 && newWidth > 0)
            {
                double ratio = newWidth / scaled_width;
                offset *= ratio;
            }

            // 设置新的图片宽度
            scaled_width = newWidth;

            // 设置新的图片尺寸
            Image1.Width = Image2.Width = scaled_width;
            Image1.Height = Image2.Height = ActualHeight;

            // 强制刷新布局
            UpdateLayout();
        }

        private void UpdatePosition(object sender, EventArgs e)
        //此函数更新图片位置
        {
            // 计算现在和上次移动图片的时间差
            TimeSpan elapsed = DateTime.Now - last_update;
            //更新“上次”移动图片的时间为现在时间
            last_update = DateTime.Now;

            // 根据时间差计算新偏移量
            offset -= speed * elapsed.TotalSeconds;

            //第一张图片左移出屏幕后(此时第二张图片左边正好对齐屏幕左侧)，立即把两张图片一起拉回原位(此时第一张图片处在刚才第二张图的位置，左边对齐屏幕左侧，而第二张图在第一张图的右侧，已经在屏幕右侧之外了)，并继续左移。但用户看来还是第二张图片在左移。
            if (offset < -scaled_width)
            {
                offset += scaled_width;
            }

            //把 _offset 的值四舍五入到最接近的整数（0 表示保留 0 位小数），然后赋值给 _transform1.X。目的是让图片的移动位置精确到像素，避免出现亚像素渲染导致的模糊或抖动现象
            transform2.X = Math.Round(offset + scaled_width - 1, 0);
            transform1.X = Math.Round(offset, 0);
        }

        #endregion



        #region 游戏交互逻辑处理函数


        private void Start_game()
        {
            //开始游戏会有两种情况，一是什么都存档都没有，则进入New_start界面创建新的存档；二是有存档，那么Option文件中必然有save_choose值，则进入Ribillion_qoutes页面。
            if (Option.save_choose == 0)
            {
                Page_frame.Navigate(new New_start());                                           //跳转到New_start界面
            }
            else
            {
                Page_frame.Navigate(new Rebillion_qoutes());                                    //跳转到Rebillion界面
            }
        }



        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (Option.tutorial_completed == 0)
            {
                Page_frame.Navigate(new Tutorial());                                           //跳转到Tutorial界面
            }
            else
            {
                if (e.Key == Key.F12)
                {
                    //F12键在MainWindow里面处理，这里不处理
                    return;
                }
                if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.Oem2))                              //输入 shift+/ 进入Tutorial界面
                {
                    Option.tutorial_completed = 0;                                                                          //重置教程完成状态
                    Page_frame.Navigate(new Tutorial());                                                                    //跳转到Tutorial界面            
                }
                if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.OemSemicolon))                      //输入 shift+; 进入Option界面
                {
                    Page_frame.Navigate(new Options());                                                                     //跳转到Option界面            
                }
                else if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.A))                            //输入 shift+a 进入Trace 界面
                {
                    Page_frame.Navigate(new Trace());                                                                       //跳转到Trace界面               
                }
                else if (e.Key == Key.Escape)                                                                  //按Esc键直接退出游戏
                {
                    //关闭整个程序
                    Data.Main_window.Close();
                }
                else if ((e.Key != Key.LeftShift) && (e.Key != Key.RightShift))                                                //按其余任意键进入游戏
                {
                    Start_game();
                }
            }
        }


        private void Page_MouseDown(object sender, MouseButtonEventArgs e)                             //点击屏幕任意区域进入游戏
        {

            if (Option.tutorial_completed == 0)
            {
                Page_frame.Navigate(new Tutorial());                                           //跳转到Tutorial界面
            }
            else
            {
                Start_game();
            }
        }

        #endregion

    }
}
