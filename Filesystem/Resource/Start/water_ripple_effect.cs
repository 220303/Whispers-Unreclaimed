using System.Windows;
using System.Windows.Media.Effects;
using System.Windows.Media;

namespace 烟尘记
{
    public class WaterRippleEffect : ShaderEffect
    {
        private static readonly PixelShader _shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Filesystem/Resource/Start/water_ripple.ps")
        };

        public WaterRippleEffect()
        {
            PixelShader = _shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(CenterProperty);
            UpdateShaderValue(TimeProperty);
            UpdateShaderValue(AmplitudeProperty);
            UpdateShaderValue(FrequencyProperty);
            UpdateShaderValue(SpeedProperty);
            UpdateShaderValue(AspectRatioProperty);
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(WaterRippleEffect), 0);

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(WaterRippleEffect),
                new UIPropertyMetadata(new Point(0.5, 0.5), PixelShaderConstantCallback(0)));

        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(float), typeof(WaterRippleEffect),
                new UIPropertyMetadata(0f, PixelShaderConstantCallback(1)));

        public float Time
        {
            get => (float)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly DependencyProperty AmplitudeProperty =
            DependencyProperty.Register("Amplitude", typeof(float), typeof(WaterRippleEffect),
                new UIPropertyMetadata(0.02f, PixelShaderConstantCallback(2)));

        public float Amplitude
        {
            get => (float)GetValue(AmplitudeProperty);
            set => SetValue(AmplitudeProperty, value);
        }

        public static readonly DependencyProperty FrequencyProperty =
            DependencyProperty.Register("Frequency", typeof(float), typeof(WaterRippleEffect),
                new UIPropertyMetadata(30f, PixelShaderConstantCallback(3)));

        public float Frequency
        {
            get => (float)GetValue(FrequencyProperty);
            set => SetValue(FrequencyProperty, value);
        }

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(float), typeof(WaterRippleEffect),
                new UIPropertyMetadata(6f, PixelShaderConstantCallback(4)));

        public float Speed
        {
            get => (float)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public static readonly DependencyProperty AspectRatioProperty =
            DependencyProperty.Register("AspectRatio", typeof(float), typeof(WaterRippleEffect),
        new UIPropertyMetadata(1f, PixelShaderConstantCallback(5)));

        public float AspectRatio
        {
            get => (float)GetValue(AspectRatioProperty);
            set => SetValue(AspectRatioProperty, value);
        }

    }
}
