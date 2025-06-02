using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 烟尘记
{
    /// <summary>
    /// Fade_boundary.xaml 的交互逻辑
    /// </summary>
    public partial class Fade_boundary : UserControl
    {
        public Fade_boundary()
        {
            InitializeComponent();
        }
        // 通用颜色属性
        public static readonly DependencyProperty EdgeColorProperty =
            DependencyProperty.Register("EdgeColor", typeof(Color), typeof(Fade_boundary),
                new PropertyMetadata(Colors.White));

        public Color EdgeColor
        {
            get => (Color)GetValue(EdgeColorProperty);
            set => SetValue(EdgeColorProperty, value);
        }

        // 每条边的 Thickness（控制边线粗细）
        public double TopEdgeThickness { get => (double)GetValue(TopEdgeThicknessProperty); set => SetValue(TopEdgeThicknessProperty, value); }
        public static readonly DependencyProperty TopEdgeThicknessProperty =
            DependencyProperty.Register("TopEdgeThickness", typeof(double), typeof(Fade_boundary), new PropertyMetadata(4.0));

        public double BottomEdgeThickness { get => (double)GetValue(BottomEdgeThicknessProperty); set => SetValue(BottomEdgeThicknessProperty, value); }
        public static readonly DependencyProperty BottomEdgeThicknessProperty =
            DependencyProperty.Register("BottomEdgeThickness", typeof(double), typeof(Fade_boundary), new PropertyMetadata(4.0));

        public double LeftEdgeThickness { get => (double)GetValue(LeftEdgeThicknessProperty); set => SetValue(LeftEdgeThicknessProperty, value); }
        public static readonly DependencyProperty LeftEdgeThicknessProperty =
            DependencyProperty.Register("LeftEdgeThickness", typeof(double), typeof(Fade_boundary), new PropertyMetadata(4.0));

        public double RightEdgeThickness { get => (double)GetValue(RightEdgeThicknessProperty); set => SetValue(RightEdgeThicknessProperty, value); }
        public static readonly DependencyProperty RightEdgeThicknessProperty =
            DependencyProperty.Register("RightEdgeThickness", typeof(double), typeof(Fade_boundary), new PropertyMetadata(4.0));

        // 每条边的渐隐控制点（0 到 1）
        public double TopEdgeFadeStart { get => (double)GetValue(TopEdgeFadeStartProperty); set => SetValue(TopEdgeFadeStartProperty, value); }
        public static readonly DependencyProperty TopEdgeFadeStartProperty =
            DependencyProperty.Register("TopEdgeFadeStart", typeof(double), typeof(Fade_boundary), new PropertyMetadata(0.0));

        public double TopEdgeFadeEnd { get => (double)GetValue(TopEdgeFadeEndProperty); set => SetValue(TopEdgeFadeEndProperty, value); }
        public static readonly DependencyProperty TopEdgeFadeEndProperty =
            DependencyProperty.Register("TopEdgeFadeEnd", typeof(double), typeof(Fade_boundary), new PropertyMetadata(1.0));

        // Bottom
        public double BottomEdgeFadeStart { get => (double)GetValue(BottomEdgeFadeStartProperty); set => SetValue(BottomEdgeFadeStartProperty, value); }
        public static readonly DependencyProperty BottomEdgeFadeStartProperty =
            DependencyProperty.Register("BottomEdgeFadeStart", typeof(double), typeof(Fade_boundary), new PropertyMetadata(0.0));

        public double BottomEdgeFadeEnd { get => (double)GetValue(BottomEdgeFadeEndProperty); set => SetValue(BottomEdgeFadeEndProperty, value); }
        public static readonly DependencyProperty BottomEdgeFadeEndProperty =
            DependencyProperty.Register("BottomEdgeFadeEnd", typeof(double), typeof(Fade_boundary), new PropertyMetadata(1.0));

        // Left
        public double LeftEdgeFadeStart { get => (double)GetValue(LeftEdgeFadeStartProperty); set => SetValue(LeftEdgeFadeStartProperty, value); }
        public static readonly DependencyProperty LeftEdgeFadeStartProperty =
            DependencyProperty.Register("LeftEdgeFadeStart", typeof(double), typeof(Fade_boundary), new PropertyMetadata(0.0));

        public double LeftEdgeFadeEnd { get => (double)GetValue(LeftEdgeFadeEndProperty); set => SetValue(LeftEdgeFadeEndProperty, value); }
        public static readonly DependencyProperty LeftEdgeFadeEndProperty =
            DependencyProperty.Register("LeftEdgeFadeEnd", typeof(double), typeof(Fade_boundary), new PropertyMetadata(1.0));

        // Right
        public double RightEdgeFadeStart { get => (double)GetValue(RightEdgeFadeStartProperty); set => SetValue(RightEdgeFadeStartProperty, value); }
        public static readonly DependencyProperty RightEdgeFadeStartProperty =
            DependencyProperty.Register("RightEdgeFadeStart", typeof(double), typeof(Fade_boundary), new PropertyMetadata(0.0));

        public double RightEdgeFadeEnd { get => (double)GetValue(RightEdgeFadeEndProperty); set => SetValue(RightEdgeFadeEndProperty, value); }
        public static readonly DependencyProperty RightEdgeFadeEndProperty =
            DependencyProperty.Register("RightEdgeFadeEnd", typeof(double), typeof(Fade_boundary), new PropertyMetadata(1.0));
    }
}
