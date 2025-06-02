using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace 烟尘记
{
    /// <summary>
    /// Trace.xaml 的交互逻辑
    /// </summary>
    public partial class Trace : Page
    {
        public Trace() //接受数字参数： 可/不可 操作   可/不可 退回
        {
            InitializeComponent();

            if (Option.save_choose != 0)                                                                                      //刚安装好游戏时默认是0，此时不高亮SelectItem(我的Style是加红框)
                Saves_ListBox.SelectedItem = Save.saves.FirstOrDefault(s => s.UUID == Option.save_choose);                    //通过高亮SelectItem(显灰)显示选中的存档

            this.Loaded += (s, e) => this.Focus();                                                                            //确保焦点汇聚到page上
        }


        private Button_animation.ParticleColorScheme Red_colors = new Button_animation.ParticleColorScheme
        {
            Gradient_start = Color.FromRgb(255, 69, 0),     // #FF4500 猩红色
            Gradient_end = Color.FromArgb(0, 255, 69, 0),   // 透明
            GlowColor = Color.FromRgb(220, 20, 60)          // #DC143C 深血红色
        };


        private void Page_KeyDown(object sender, KeyEventArgs e)
        {

            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.A))
            {
                Page_frame.Go_back();
            }

        }




        private void Saves_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Saves_ListBox.SelectedItem != null)                                                           //如果选中了某个存档，则将Option.save_choose设置为该存档的UUID，并立即写入Option文件
            {
                Option.save_choose = ((Save.save)Saves_ListBox.SelectedItem).UUID;
                Option.Wrtie_out();
            }

        }



        private async void Delete_save_Click(object sender, RoutedEventArgs e)
        {
            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Red_colors);

            if (Saves_ListBox.SelectedItem != null)
            {
                Save.save selected = (Save.save)Saves_ListBox.SelectedItem;
                if (Option.save_choose == selected.UUID)                                        //如果删除的存档是当前选中的存档，则将Option.save_choose设置为0，并立即写入Option文件
                {
                    Option.save_choose = 0;
                    Option.Wrtie_out();
                }
                Save.saves.Remove(selected);                                                   //从Save.saves中删除选中的存档
                File.Delete(Save.Saves_directory_path + selected.Name + ".txt");               //删除对应的存档文件
            }

           //恢复按钮响应
           ((Button)sender).IsEnabled = true;
        }

        private async void New_save_Click(object sender, RoutedEventArgs e)
        {
            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Red_colors);

            Page_frame.Navigate(new New_start());           //跳转到New_start界面

            //恢复按钮响应
            ((Button)sender).IsEnabled = true;
        }


    }
}


/*
 *  <Style TargetType="Button">
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Grid>
                            <!-- 背景层 - 增强光晕 -->
                            <Border
                        x:Name="BackgroundBorder"
                        Background="{TemplateBinding Background}"
                        BorderBrush="#8A0707"
                        BorderThickness="2"
                        CornerRadius="4">
                                <Border.Effect>
                                    <DropShadowEffect
                                x:Name="BorderGlow"
                                BlurRadius="0"
                                Opacity="0"
                                ShadowDepth="0"
                                Color="#FFF77777" />
                                </Border.Effect>
                            </Border>

                            <!-- 内容层 -->
                            <TextBlock
                        x:Name="ButtonText"
                        HorizontalAlignment="Center"
                        VerticalAlignment="Center"
                        Effect="{x:Null}"
                        FontFamily="Palatino Linotype"
                        FontSize="18"
                        FontWeight="Bold"
                        Foreground="White"
                        Text="{TemplateBinding Content}" />

                            <!-- 粒子效果层 -->
                            <Canvas x:Name="ParticleCanvas" IsHitTestVisible="False" />
                        </Grid>

                        <ControlTemplate.Triggers>
                            <!-- 鼠标悬停动画 -->
                            <Trigger Property="IsMouseOver" Value="True">
                                <Trigger.EnterActions>
                                    <BeginStoryboard>
                                        <Storyboard>
                                            <ColorAnimation
                                        Storyboard.TargetName="ButtonText"
                                        Storyboard.TargetProperty="Foreground.Color"
                                        To="#FF4500"
                                        Duration="0:0:1" />
                                            <DoubleAnimation
                                        Storyboard.TargetName="BorderGlow"
                                        Storyboard.TargetProperty="BlurRadius"
                                        To="25"
                                        Duration="0:0:1" />
                                            <DoubleAnimation
                                        Storyboard.TargetName="BorderGlow"
                                        Storyboard.TargetProperty="Opacity"
                                        To="1.0"
                                        Duration="0:0:1" />
                                            <ObjectAnimationUsingKeyFrames Storyboard.TargetName="ButtonText" Storyboard.TargetProperty="Effect">
                                                <DiscreteObjectKeyFrame KeyTime="0:0:0" Value="{x:Null}" />
                                                <DiscreteObjectKeyFrame KeyTime="0:0:1">
                                                    <DiscreteObjectKeyFrame.Value>
                                                        <DropShadowEffect
                                                    BlurRadius="12"
                                                    Opacity="0.8"
                                                    ShadowDepth="0"
                                                    Color="#FF6347" />
                                                    </DiscreteObjectKeyFrame.Value>
                                                </DiscreteObjectKeyFrame>
                                            </ObjectAnimationUsingKeyFrames>
                                        </Storyboard>
                                    </BeginStoryboard>
                                </Trigger.EnterActions>

                                <Trigger.ExitActions>
                                    <BeginStoryboard>
                                        <Storyboard>
                                            <ColorAnimation
                                        Storyboard.TargetName="ButtonText"
                                        Storyboard.TargetProperty="Foreground.Color"
                                        To="White"
                                        Duration="0:0:1" />
                                            <DoubleAnimation
                                        Storyboard.TargetName="BorderGlow"
                                        Storyboard.TargetProperty="BlurRadius"
                                        To="0"
                                        Duration="0:0:1" />
                                            <DoubleAnimation
                                        Storyboard.TargetName="BorderGlow"
                                        Storyboard.TargetProperty="Opacity"
                                        To="0"
                                        Duration="0:0:1" />
                                            <ObjectAnimationUsingKeyFrames Storyboard.TargetName="ButtonText" Storyboard.TargetProperty="Effect">
                                                <DiscreteObjectKeyFrame KeyTime="0:0:0">
                                                    <DiscreteObjectKeyFrame.Value>
                                                        <DropShadowEffect
                                                    BlurRadius="12"
                                                    Opacity="0.8"
                                                    ShadowDepth="0"
                                                    Color="#FF6347" />
                                                    </DiscreteObjectKeyFrame.Value>
                                                </DiscreteObjectKeyFrame>
                                                <DiscreteObjectKeyFrame KeyTime="0:0:1" Value="{x:Null}" />
                                            </ObjectAnimationUsingKeyFrames>
                                        </Storyboard>
                                    </BeginStoryboard>
                                </Trigger.ExitActions>
                            </Trigger>

                            <!-- 按下效果 -->
                            <Trigger Property="IsPressed" Value="True">
                                <Setter TargetName="BackgroundBorder" Property="BorderBrush" Value="#B22222" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>

            <Setter Property="Background">
                <Setter.Value>
                    <SolidColorBrush Color="Black" />
                </Setter.Value>
            </Setter>

            <Setter Property="Padding" Value="15,8" />

            <Setter Property="TextElement.Foreground">
                <Setter.Value>
                    <SolidColorBrush Color="White" />
                </Setter.Value>
            </Setter>
        </Style>
*/