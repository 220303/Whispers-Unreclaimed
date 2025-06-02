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
    /// Page_concer_pattern.xaml 的交互逻辑
    /// </summary>
    public partial class Page_concer_pattern : UserControl
    {
        public Page_concer_pattern()
        {
            InitializeComponent();
        }

        // 支持外部设置的角落图案资源
        public static readonly DependencyProperty CornerImageSourceProperty =
            DependencyProperty.Register("CornerImageSource", typeof(ImageSource), typeof(Page_concer_pattern));

        public ImageSource CornerImageSource
        {
            get => (ImageSource)GetValue(CornerImageSourceProperty);
            set => SetValue(CornerImageSourceProperty, value);
        }

        // 设置基准参考尺寸，默认为 1920x1080 (16:9)
        private const double ReferenceWidth = 1920.0;
        private const double ReferenceHeight = 1080.0;

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double actualWidth = e.NewSize.Width;
            double actualHeight = e.NewSize.Height;

            // 当前尺寸与参考尺寸的比例
            double scaleW = actualWidth / ReferenceWidth;
            double scaleH = actualHeight / ReferenceHeight;

            // 当前宽高比
            double actualAspect = actualWidth / actualHeight;
            double referenceAspect = ReferenceWidth / ReferenceHeight;

            double finalScale;

            if (Math.Abs(actualAspect - referenceAspect) < 0.01)
            {
                // 比例一致，直接等比缩放
                finalScale = scaleW;
            }
            else
            {
                // 比例不同，取限制方向的缩放值
                finalScale = Math.Min(scaleW, scaleH);
            }

            ApplyScale(finalScale);
        }

        private void ApplyScale(double scale)
        {
            TopLeftScale.ScaleX = scale;
            TopLeftScale.ScaleY = scale;
            TopRightScale.ScaleX = scale;
            TopRightScale.ScaleY = scale;
            BottomLeftScale.ScaleX = scale;
            BottomLeftScale.ScaleY = scale;
            BottomRightScale.ScaleX = scale;
            BottomRightScale.ScaleY = scale;
        }

    }
}
