namespace 烟尘记
{

    #region UI、交互类

    public class MagicCornerBorder_Rebillion_qoutes : Canvas
    {
        // 可配置属性
        public double MarginFromEdge { get; set; } = 40;
        public double OffsetFromCorner { get; set; } = 20;
        public Brush StrokeBrush { get; set; } = Brushes.White;
        public double StrokeThickness { get; set; } = 2;

        private const double PatternSize = 80;
        private const double PatternScale = 1.0;

        // 角图案几何缓存
        private static readonly Geometry CornerGeometry = CreateCornerGeometry();

        /// <summary>
        /// 重绘控件时绘制四角装饰和边框。
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            double width = ActualWidth;
            double height = ActualHeight;

            var pen = new Pen(StrokeBrush, StrokeThickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };

            // 四角中心点
            var corners = new[]
            {
                new Point(MarginFromEdge + OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner),
                new Point(MarginFromEdge + OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner)
            };

            // 绘制四角图案
            DrawCornerPattern(dc, corners[0], false, false);
            DrawCornerPattern(dc, corners[1], true, false);
            DrawCornerPattern(dc, corners[2], true, true);
            DrawCornerPattern(dc, corners[3], false, true);

            // 计算边缘位置
            double half = PatternSize / 2;
            double leftX = MarginFromEdge + OffsetFromCorner - half;
            double rightX = width - MarginFromEdge - OffsetFromCorner + half;
            double topY = MarginFromEdge + OffsetFromCorner - half;
            double bottomY = height - MarginFromEdge - OffsetFromCorner + half;

            // 绘制四条边
            DrawFadingLineSymmetric(dc, new Point(leftX + PatternSize, topY), new Point(rightX - PatternSize, topY));
            DrawFadingLineSymmetric(dc, new Point(leftX + PatternSize, bottomY), new Point(rightX - PatternSize, bottomY));
            DrawFadingLineSymmetric(dc, new Point(leftX, topY + PatternSize), new Point(leftX, bottomY - PatternSize));
            DrawFadingLineSymmetric(dc, new Point(rightX, topY + PatternSize), new Point(rightX, bottomY - PatternSize));
        }

        /// <summary>
        /// 绘制带渐变的边线（对称两段）。
        /// </summary>
        private void DrawFadingLineSymmetric(DrawingContext dc, Point start, Point end)
        {
            Point mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            DrawFadingLine(dc, start, mid);
            DrawFadingLine(dc, end, mid);
        }

        /// <summary>
        /// 绘制一段带渐变的线。
        /// </summary>
        private void DrawFadingLine(DrawingContext dc, Point from, Point to)
        {
            var gradient = new LinearGradientBrush(Colors.White, Colors.Transparent, 0.0)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                RelativeTransform = new RotateTransform(
                    Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI, 0.5, 0.5)
            };
            var pen = new Pen(gradient, StrokeThickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            dc.DrawLine(pen, from, to);
        }

        /// <summary>
        /// 绘制单个角的装饰图案。
        /// </summary>
        private void DrawCornerPattern(DrawingContext dc, Point center, bool flipX, bool flipY)
        {
            var transform = new TransformGroup();
            transform.Children.Add(new TranslateTransform(-PatternSize * PatternScale / 2, -PatternSize * PatternScale / 2));
            transform.Children.Add(new ScaleTransform(flipX ? -1 : 1, flipY ? -1 : 1));
            transform.Children.Add(new TranslateTransform(center.X, center.Y));

            var geometry = CornerGeometry.Clone();
            geometry.Transform = transform;

            dc.DrawGeometry(null, new Pen(StrokeBrush, StrokeThickness), geometry);
        }

        /// <summary>
        /// 创建角装饰的几何图形。
        /// </summary>
        private static Geometry CreateCornerGeometry()
        {
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                DrawLine(ctx, 0, 80, 0, 20);
                DrawLine(ctx, 0, 20, 20, 0);
                DrawLine(ctx, 20, 0, 80, 0);
                DrawLine(ctx, 0, 60, 60, 0);
                DrawLine(ctx, 10, 80, 80, 10);
                DrawLine(ctx, 20, 80, 80, 20);
                DrawLine(ctx, 0, 40, 40, 0);
            }
            geometry.Freeze();
            return geometry;
        }

        /// <summary>
        /// 绘制一条线段。
        /// </summary>
        private static void DrawLine(StreamGeometryContext ctx, double x1, double y1, double x2, double y2)
        {
            ctx.BeginFigure(new Point(x1, y1), false, false);
            ctx.LineTo(new Point(x2, y2), true, false);
        }

        /// <summary>
        /// 尺寸变化时重绘。
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
    }

    public class MagicCornerBorder_Trace : Canvas
    {
        // 可配置属性
        public double MarginFromEdge { get; set; } = 40;
        public double OffsetFromCorner { get; set; } = 20;
        public double StrokeThickness { get; set; } = 2;

        private const double PatternSize = 80;

        /// <summary>
        /// 重绘控件时绘制四角装饰和边框。
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            double width = ActualWidth;
            double height = ActualHeight;

            // 四角中心点
            var corners = new[]
            {
                new Point(MarginFromEdge + OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner),
                new Point(MarginFromEdge + OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner)
            };

            // 计算边缘位置
            double half = PatternSize / 2;
            double leftX = MarginFromEdge + OffsetFromCorner - half;
            double rightX = width - MarginFromEdge - OffsetFromCorner + half;
            double topY = MarginFromEdge + OffsetFromCorner - half;
            double bottomY = height - MarginFromEdge - OffsetFromCorner + half;

            // 绘制四条边
            DrawFadingLineSymmetric(dc, new Point(leftX, topY), new Point(rightX, topY));
            DrawFadingLineSymmetric(dc, new Point(leftX, bottomY), new Point(rightX, bottomY));
            DrawFadingLineSymmetric(dc, new Point(leftX, topY), new Point(leftX, bottomY));
            DrawFadingLineSymmetric(dc, new Point(rightX, topY), new Point(rightX, bottomY));
        }

        /// <summary>
        /// 绘制带渐变的边线（对称两段）。
        /// </summary>
        private void DrawFadingLineSymmetric(DrawingContext dc, Point start, Point end)
        {
            Point mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            DrawFadingLine(dc, start, mid);
            DrawFadingLine(dc, end, mid);
        }

        /// <summary>
        /// 绘制一段带渐变的线。
        /// </summary>
        private void DrawFadingLine(DrawingContext dc, Point from, Point to)
        {
            var gradient = new LinearGradientBrush(Colors.DarkRed, Colors.Transparent, 0.0)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                RelativeTransform = new RotateTransform(
                    Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI, 0.5, 0.5)
            };
            var pen = new Pen(gradient, StrokeThickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            dc.DrawLine(pen, from, to);
        }


        /// <summary>
        /// 尺寸变化时重绘。
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
    }
    public class MagicCornerBorder_Option : Canvas
    {
        // 可配置属性
        public double MarginFromEdge { get; set; } = 40;
        public double OffsetFromCorner { get; set; } = 20;
        public double StrokeThickness { get; set; } = 2;

        private const double PatternSize = 80;

        /// <summary>
        /// 重绘控件时绘制四角装饰和边框。
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            double width = ActualWidth;
            double height = ActualHeight;

            // 四角中心点
            var corners = new[]
            {
                new Point(MarginFromEdge + OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner),
                new Point(MarginFromEdge + OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner)
            };

            // 计算边缘位置
            double half = PatternSize / 2;
            double leftX = MarginFromEdge + OffsetFromCorner - half;
            double rightX = width - MarginFromEdge - OffsetFromCorner + half;
            double topY = MarginFromEdge + OffsetFromCorner - half;
            double bottomY = height - MarginFromEdge - OffsetFromCorner + half;

            // 绘制四条边
            DrawFadingLineSymmetric(dc, new Point(leftX, topY), new Point(rightX, topY));
            DrawFadingLineSymmetric(dc, new Point(leftX, bottomY), new Point(rightX, bottomY));
            DrawFadingLineSymmetric(dc, new Point(leftX, topY), new Point(leftX, bottomY));
            DrawFadingLineSymmetric(dc, new Point(rightX, topY), new Point(rightX, bottomY));
        }

        /// <summary>
        /// 绘制带渐变的边线（对称两段）。
        /// </summary>
        private void DrawFadingLineSymmetric(DrawingContext dc, Point start, Point end)
        {
            Point mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            DrawFadingLine(dc, start, mid);
            DrawFadingLine(dc, end, mid);
        }

        /// <summary>
        /// 绘制一段带渐变的线。
        /// </summary>
        private void DrawFadingLine(DrawingContext dc, Point from, Point to)
        {
            var gradient = new LinearGradientBrush(Colors.DarkGreen, Colors.Transparent, 0.0)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                RelativeTransform = new RotateTransform(
                    Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI, 0.5, 0.5)
            };
            var pen = new Pen(gradient, StrokeThickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            dc.DrawLine(pen, from, to);
        }


        /// <summary>
        /// 尺寸变化时重绘。
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
    }
    public class MagicCornerBorder_Pause : Canvas
    {
        // 可配置属性
        public double MarginFromEdge { get; set; } = 40;
        public double OffsetFromCorner { get; set; } = 20;
        public double StrokeThickness { get; set; } = 2;

        private const double PatternSize = 80;

        /// <summary>
        /// 重绘控件时绘制四角装饰和边框。
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            double width = ActualWidth;
            double height = ActualHeight;

            // 四角中心点
            var corners = new[]
            {
                new Point(MarginFromEdge + OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, MarginFromEdge + OffsetFromCorner),
                new Point(width - MarginFromEdge - OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner),
                new Point(MarginFromEdge + OffsetFromCorner, height - MarginFromEdge - OffsetFromCorner)
            };

            // 计算边缘位置
            double half = PatternSize / 2;
            double leftX = MarginFromEdge + OffsetFromCorner - half;
            double rightX = width - MarginFromEdge - OffsetFromCorner + half;
            double topY = MarginFromEdge + OffsetFromCorner - half;
            double bottomY = height - MarginFromEdge - OffsetFromCorner + half;

            // 绘制四条边
            DrawFadingLineSymmetric(dc, new Point(leftX, topY), new Point(rightX, topY));
            DrawFadingLineSymmetric(dc, new Point(leftX, bottomY), new Point(rightX, bottomY));
            DrawFadingLineSymmetric(dc, new Point(leftX, topY), new Point(leftX, bottomY));
            DrawFadingLineSymmetric(dc, new Point(rightX, topY), new Point(rightX, bottomY));
        }

        /// <summary>
        /// 绘制带渐变的边线（对称两段）。
        /// </summary>
        private void DrawFadingLineSymmetric(DrawingContext dc, Point start, Point end)
        {
            Point mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            DrawFadingLine(dc, start, mid);
            DrawFadingLine(dc, end, mid);
        }

        /// <summary>
        /// 绘制一段带渐变的线。
        /// </summary>
        private void DrawFadingLine(DrawingContext dc, Point from, Point to)
        {
            var gradient = new LinearGradientBrush(Colors.DarkBlue, Colors.Transparent, 0.0)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                RelativeTransform = new RotateTransform(
                    Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI, 0.5, 0.5)
            };
            var pen = new Pen(gradient, StrokeThickness)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            dc.DrawLine(pen, from, to);
        }


        /// <summary>
        /// 尺寸变化时重绘。
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
    }




    public class ParticleCanvas : Canvas
    {
        private class Particle
        {
            public Point Position;
            public Vector Direction;
            public double Opacity;
            public double Size;
            public double Life;
            public double Age;
            public Color ColorStart;
            public Color ColorEnd;
        }

        private readonly List<Particle> particles = new();
        private readonly Random rand = new();
        private bool animating = false;
        private double durationSeconds = 1.5;

        // 新增参数 maxDistance, durationSeconds
        public void StartParticles(int count, Color colorStart, Color colorEnd, Rect buttonRect, double maxDistance, double duration)
        {
            particles.Clear();
            durationSeconds = duration;
            for (int i = 0; i < count; i++)
            {
                var start = GetRandomPointOnBorder(buttonRect, rand);
                var angle = rand.NextDouble() * Math.PI * 2;
                var distance = rand.NextDouble() * maxDistance * 0.5 + maxDistance * 0.5;
                var direction = new Vector(Math.Cos(angle), Math.Sin(angle)) * (distance / durationSeconds * 0.01); //速度，0.03，越大越快

                particles.Add(new Particle
                {
                    Position = start,
                    Direction = direction,
                    Opacity = 0.7,
                    Size = rand.Next(3, 6),
                    Life = durationSeconds,
                    Age = 0,
                    ColorStart = colorStart,
                    ColorEnd = colorEnd
                });
            }
            if (!animating)
            {
                CompositionTarget.Rendering += OnRendering;
                animating = true;
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            foreach (var p in particles)
            {
                p.Age += 0.02;
                p.Position += p.Direction;
                p.Opacity = 0.7 * (1 - p.Age / p.Life);
            }
            particles.RemoveAll(p => p.Age >= p.Life);
            InvalidateVisual();
            if (particles.Count == 0)
            {
                CompositionTarget.Rendering -= OnRendering;
                animating = false;
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            foreach (var p in particles)
            {
                var brush = new RadialGradientBrush(p.ColorStart, p.ColorEnd) { Opacity = p.Opacity };
                dc.DrawEllipse(brush, null, p.Position, p.Size, p.Size);
            }
        }

        private Point GetRandomPointOnBorder(Rect rect, Random rand)
        {
            int side = rand.Next(0, 4);
            double x = 0, y = 0;
            switch (side)
            {
                case 0: x = rand.NextDouble() * rect.Width; y = rect.Top; break;
                case 1: x = rect.Right; y = rand.NextDouble() * rect.Height; break;
                case 2: x = rand.NextDouble() * rect.Width; y = rect.Bottom; break;
                case 3: x = rect.Left; y = rand.NextDouble() * rect.Height; break;
            }
            return new Point(x, y);
        }
    }
    public static class Button_animation
    {
        public struct ParticleColorScheme
        {
            public Color Gradient_start;     // 粒子渐变开始颜色
            public Color Gradient_end;       // 粒子渐变结束颜色（通常是透明）
            public Color GlowColor;          // 发光颜色
        }




        public static async Task Animation(Button button, ParticleColorScheme colors)
        {
            var canvas = button.Template.FindName("ParticleCanvas", button) as ParticleCanvas;
            if (canvas == null) return;

            // 获取按钮实际区域
            var border = button.Template.FindName("BackgroundBorder", button) as Border;
            if (border == null) return;
            Rect buttonRect = new Rect(0, 0, button.ActualWidth, button.ActualHeight);

            int particleCount = (int)((button.ActualWidth + button.ActualHeight) * 1.1); // 数量和按钮大小相关，0.8，越大越大
            double maxDistance = Math.Max(buttonRect.Width, buttonRect.Height) * 0.2;    // 扩散范围和按钮大小相关，0.5，越大越大
            double durationSeconds = Math.Max(1.5, maxDistance / 400.0);                 // 动画时长和扩散范围相关，400，越大越慢

            canvas.StartParticles(particleCount, colors.Gradient_start, colors.Gradient_end, buttonRect, maxDistance, durationSeconds);

            //耐心等待按钮的粒子动画完成
            await Task.Delay((int)(durationSeconds * 1000));
        }

    }


    public static class Music_player
    {
        //存放背景音乐的位置
        public static string Music_directory_path = Data.Filesystem_directory_path + "Resource/music/";


        //保存全局音乐播放器
        public static MediaElement music_player;

        //当前页面所属音乐播放组的记录
        public static string Current_music_page_group;



        public static void Start(string Current_music_page_group)
        {

            //初始化全局音频播放器
            music_player = Data.Main_window.global_media_element;

            //设置开始时的音频组标识
            Current_music_page_group = Current_music_page_group;

            //设置音频播放器播放游戏背景音乐
            music_player.Source = new Uri(Music_directory_path + "程序背景音.mp3", UriKind.RelativeOrAbsolute);

            //设置音量
            music_player.Volume = Option.music_volume;

            //音频播放器开始播放
            music_player.Play();

            //设置音频循环播放
            music_player.MediaEnded += (s, e) =>
            {
                music_player.Position = TimeSpan.Zero;
                music_player.Play();
            };

            // 监听Frame导航来更改音乐
            Data.Main_window.page_frame.Navigated += Change_music;
        }

        private static void Change_music(object sender, NavigationEventArgs e)
        {

            // 创建一个 DispatcherTimer
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // 每 50 毫秒检查一次
            };

            // 定义检查逻辑
            timer.Tick += (s, args) =>
            {
                // 检查 e.Content 是否为 Page 类型
                if (e.Content is Page page)
                {
                    // 停止计时器
                    timer.Stop();

                    string new_music_page_group = ((Page)e.Content).Tag?.ToString();                  // 从页面Tag获取音频组标识

                    if ((new_music_page_group != Current_music_page_group) && (new_music_page_group != "all"))
                    {
                        // 停止当前音频
                        music_player.Stop();
                        // 根据新的音频组标识设置对应的音频
                        switch (new_music_page_group)
                        {
                            case "program":
                                music_player.Source = new Uri(Music_directory_path + "程序背景音.mp3", UriKind.RelativeOrAbsolute);
                                break;
                            case "game":
                                music_player.Source = new Uri(Music_directory_path + "游戏背景音.mp3", UriKind.RelativeOrAbsolute);
                                break;
                        }

                        // 播放新音频
                        music_player.Play();
                        //更改当前页面分组的记录
                        Current_music_page_group = new_music_page_group;
                    }

                }
            };

            // 启动计时器
            timer.Start();
        }

    }

    public static class Page_frame
    {

        //类内部防抖变量
        private static bool Navigate_permit;


        //存放对Mainwindow的动画蒙版对象的引用
        private static Rectangle black_mask;

        //存放对Mainwindow的Frame对象的引用
        private static Frame page_frame;


        //页面切入和切出的导航动画
        private static Storyboard Page_out_storyboard;
        private static Storyboard Page_in_storyboard;


        public static void Navigation_animation_Initialize(Frame page_frame, Rectangle black_mask)
        //导航动画初始化函数
        {
            #region 初始化导航类内容

            Page_frame.page_frame = page_frame;
            Page_frame.black_mask = black_mask;
            Page_frame.Navigate_permit = true;

            #endregion

            #region 初始化两个动画故事板

            Page_out_storyboard = new Storyboard();
            var outAnim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.8)
            };
            Storyboard.SetTargetProperty(outAnim, new PropertyPath("Opacity"));
            Page_out_storyboard.Children.Add(outAnim);

            Page_in_storyboard = new Storyboard();
            var inAnim = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.8)
            };
            Storyboard.SetTargetProperty(inAnim, new PropertyPath("Opacity"));
            Page_in_storyboard.Children.Add(inAnim);

            #endregion
        }


        public static Task AwaitStoryboardAsync(Storyboard storyboard)
        //把故事板动画封装为可等待的Task(当然，直接使用了black_mask引用)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (storyboard == null)
            {
                tcs.SetResult(false);
                return tcs.Task;
            }

            void OnCompleted(object sender, EventArgs e)
            {
                storyboard.Completed -= OnCompleted;
                tcs.SetResult(true);
            }

            storyboard.Completed += OnCompleted;
            storyboard.Begin(black_mask);

            return tcs.Task;
        }


        public async static void Navigate(object content)
        //重写导航逻辑
        {
            if (Navigate_permit)
            {
                #region 增加防抖

                //增加类内部防抖
                Navigate_permit = false;

                //将蒙版设置为可见，同时也就全局性阻止了鼠标输入
                black_mask.Visibility = Visibility.Visible;

                #endregion

                //播放切出页面动画
                await AwaitStoryboardAsync(Page_out_storyboard);

                //执行实际导航到新页面
                page_frame.Navigate(content);

                //播放切入页面动画
                await AwaitStoryboardAsync(Page_in_storyboard);

                #region 解除防抖


                // 隐藏蒙版，同时也就恢复了鼠标输入
                black_mask.Visibility = Visibility.Collapsed;

                //解除类内部防抖
                Navigate_permit = true;

                #endregion

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("防抖系统发挥作用，导航到新页面被阻止。");
            }
        }

        public async static void Go_back()
        //重写导航逻辑
        {
            if (Navigate_permit)
            {
                #region 增加防抖

                //增加类内部防抖
                Navigate_permit = false;

                //将蒙版设置为可见，同时也就全局性阻止了鼠标输入
                black_mask.Visibility = Visibility.Visible;

                #endregion

                //播放切出页面动画
                await AwaitStoryboardAsync(Page_out_storyboard);

                //执行实际导航返回上一页面
                page_frame.GoBack();

                //播放切入页面动画
                await AwaitStoryboardAsync(Page_in_storyboard);

                #region 解除防抖


                // 隐藏蒙版，同时也就恢复了鼠标输入
                black_mask.Visibility = Visibility.Collapsed;

                //解除类内部防抖
                Navigate_permit = true;

                #endregion

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("防抖系统发挥作用，导航返回上一页面被阻止。");
            }
        }

    }
    #endregion



    #region 全局数据静态类

    public static class Data
    {
        //游戏所有文件的根目录
        public static string Filesystem_directory_path = "./Filesystem/";

        //存放背景图片的位置
        public static string Image_directory_path = Filesystem_directory_path + "Resource/image/";

        //为MainWindow生成的一个全局对象，使得在其他Page中也能访问到MainWindow中的内容，比如通过MainWindow.Page_frame来切换页面来切换页面
        public static MainWindow Main_window;
    }

    public static class Rebillion_qoute      //Rebillion_qoute没有s是因为要避让Rebillion_qoutes这个Page类
    {
        //存放Rebillion Qoutes的位置
        public static string Rebillion_qoutes_file_path = Data.Filesystem_directory_path + "Rebillion Qoutes.txt";

        public static string[] rebillion_qoutes = null;

        public static void Read_in()
        {
            string qoutescontent = File.ReadAllText(Rebillion_qoutes_file_path);
            rebillion_qoutes = qoutescontent.Split('\n');
        }
    }

    public static class Save
    {
        //存放存档的位置
        public static string Saves_directory_path = Data.Filesystem_directory_path + "Saves/";

        public struct save
        {
            public int UUID;

            //存档创建时间（字段+属性）
            private DateTime save_creation_time;
            public string Save_creation_time
            {
                get => save_creation_time.ToString("yyyy-MM-dd HH:mm:ss");
                set => save_creation_time = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }

            public string Name { get; set; }
            public int Jump;
            public List<string> Record;
        }

        public static ObservableCollection<save> saves = new();


        public static void Read_in()
        {

            string[] save_files = Directory.GetFiles(Saves_directory_path);
            foreach (string file in save_files)
            {
                save save = new();

                string savecontent = File.ReadAllText(file);                                                                //创建一个字符串变量用于储存从存档中读取的数据
                string[] content_array = savecontent.Split('\n');                                                           //创建数组content_array，通过换行符分开存档中不同的数据，并存入数组content_array

                save.Name = System.IO.Path.GetFileName(file).Remove(System.IO.Path.GetFileName(file).Length - 4);           //存档名即为存档文件名(去掉.txt后缀)
                save.UUID = ToInt32(content_array[0]);                                                                      //UUID
                save.Save_creation_time = content_array[1];                                                                 //Save_creation_time
                save.Jump = ToInt16(content_array[2]);                                                                      //Jump
                save.Record = new List<string>(content_array[3].Split(' '));                                                //Record

                saves.Add(save);
            }
        }

        public static void Write_out()
        {
            foreach (save save in saves)
            {
                //创建数组content_array
                string[] content_array = new string[]
                {
                Convert.ToString(save.UUID),                                          //将UUID存入数组的第一个元素
                save.Save_creation_time,                                              //将Save_creation_time存入数组的第二个元素
                Convert.ToString(save.Jump),                                          //将Jump存入数组的第三个元素
                string.Join(' ', save.Record)                                        //将Record存入数组的第四个元素（Record是一个List<string>，需要转换为字符串）
                };

                //将数据重新组合成一个字符串，并用换行符分隔每个元素，然后覆写掉存档内原有内容
                File.WriteAllText(Saves_directory_path + save.Name + ".txt", string.Join('\n', content_array));
            }
        }
    }

    public static class Option
    {

        //存放Option的位置
        public static string Options_file_path = Data.Filesystem_directory_path + "Option.txt";

        //Option的内容
        public static int save_choose { get; set; }           //注意，0意味着进入New_save界面创建新的存档
        public static double plot_font_size { get; set; }
        public static int plot_print_speed { get; set; }
        public static double music_volume { get; set; }
        public static int start_picture_speed { get; set; }
        public static int tutorial_completed { get; set; }              //教程是否完成
        public static bool credit_completed { get; set; }                     //Credit是否可跳过


        //Option的方法
        public static void Read_in()
        {
            string savecontent = File.ReadAllText(Options_file_path);                               //从设置文件中读取所有内容

            string[] content_array = savecontent.Split('\n');

            save_choose = ToInt16(content_array[0]);
            plot_font_size = ToDouble(content_array[1]);
            plot_print_speed = ToInt16(content_array[2]);
            music_volume = ToDouble(content_array[3]);
            start_picture_speed = ToInt16(content_array[4]);
            tutorial_completed = ToInt16(content_array[5]);
            credit_completed = ToBoolean(ToInt16(content_array[6]));

        }

        public static void Wrtie_out()
        {
            string[] content_array =
            [
                Convert.ToString(save_choose),
                Convert.ToString(plot_font_size),
                Convert.ToString(plot_print_speed),
                Convert.ToString(music_volume),
                Convert.ToString(start_picture_speed),
                Convert.ToString(tutorial_completed),
                Convert.ToString(credit_completed),
            ];

            File.WriteAllText(Options_file_path, string.Join('\n', content_array));                            //将数据重新组合成一个字符串，然后覆写掉设置文件内原有内容
        }
    }

    #endregion

}
