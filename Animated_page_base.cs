using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace 烟尘记
{
    public abstract class Animated_page_base : Page
    {
        // 蒙版元素（通过代码动态创建）
        private Rectangle _blackMask;

        protected Animated_page_base()
        {

            // 将原有内容包裹在 Grid 中
            var root_grid = new Grid();
            if (Content != null)
            {
                // 转移原有内容
                if (Content is UIElement element)
                {
                    root_grid.Children.Add(element);
                }
                else
                {
                    root_grid.Children.Add(new ContentControl { Content = Content });
                }
            }

            // 动态创建蒙版层并添加到页面
            _blackMask = new Rectangle
            {
                Fill = Brushes.Black,
                Opacity = 0,
                IsHitTestVisible = false, // 防止阻塞交互
                HorizontalAlignment = HorizontalAlignment.Stretch, // 横向拉伸
                VerticalAlignment = VerticalAlignment.Stretch,     // 纵向拉伸
                Width = double.NaN,  // Auto
                Height = double.NaN  // Auto
            };

            // 设置 ZIndex 为无限大来保证蒙版一定在最上面
            Panel.SetZIndex(_blackMask, int.MaxValue);


            // 监听生命周期事件
            Loaded += OnPageLoaded;
            Unloaded += OnPageUnloaded;

            Loaded += (s, e) => this.Focus();                                                               //确保焦点汇聚到page上
        }

        // 进入动画（从黑到透明）
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            PlayAnimation(_blackMask, 1, 0, TimeSpan.FromSeconds(0.5));
        }

        // 切出动画（从透明到黑）
        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            // 取消事件订阅，防止内存泄漏
            Loaded -= OnPageLoaded;
            Unloaded -= OnPageUnloaded;

            PlayAnimation(_blackMask, 0, 1, TimeSpan.FromSeconds(0.5));
        }

        // 通用动画方法
        private void PlayAnimation(UIElement target, double from, double to, TimeSpan duration)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            // 动画完成时强制设置最终状态
            animation.Completed += (s, e) =>
            {
                target.Opacity = to;
            };

            target.BeginAnimation(OpacityProperty, animation);
        }
    }

}
