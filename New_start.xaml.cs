using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace 烟尘记
{
    /// <summary>
    /// New_start.xaml 的交互逻辑
    /// </summary>
    public partial class New_start : Page
    {
        public New_start()
        {
            InitializeComponent();
            Image_update_dimensions(); //调用图片尺寸更新函数

            //设置窗口大小改变时的调用UpdateDimensions()
            SizeChanged += (s, e) => Image_update_dimensions();
        }

        private Button_animation.ParticleColorScheme Red_colors = new Button_animation.ParticleColorScheme
        {
            Gradient_start = Color.FromRgb(255, 69, 0),     // #FF4500 猩红色
            Gradient_end = Color.FromArgb(0, 255, 69, 0),   // 透明
            GlowColor = Color.FromRgb(220, 20, 60)          // #DC143C 深血红色
        };

        #region UI相关

        private void Image_update_dimensions()
        //此函数处理改变窗口大小后图片的大小及位置变化
        {
            //如果当前窗口高度小于等于0，则不进行任何操作
            if (ActualHeight <= 0) return;

            // 计算图片新的宽度(按照 48:9 的原始比例)
            double newWidth = ActualHeight * (48.0 / 9.0);

            // 设置新的图片尺寸
            //BackgroundImage.Width = newWidth;

            // 强制刷新布局
            UpdateLayout();
        }

        #endregion

        #region 交互逻辑

        private async void New_save_Click(object sender, RoutedEventArgs e)
        {
            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Red_colors);

            if (!(string.IsNullOrEmpty(New_save_name.Text)) && !(string.IsNullOrWhiteSpace(New_save_name.Text)))
            {

                int New_save_UUID;

                //查找第一个未被占用的UUID，并将其赋值给New_save_UUID。
                int i = 1;          //因为UUID从1开始，所以这里初始化为1
                while (true)
                {
                    if (!Save.saves.Any(s => s.UUID == i))  //查找到第一个未被占用的UUID
                    {
                        New_save_UUID = i;    //如果找到未被占用的UUID，则将其赋值给New_save_UUID
                        break;               //跳出循环
                    }
                    i++;
                }

                Save.saves.Add
                (
                    new Save.save()
                    {
                        UUID = New_save_UUID,
                        Save_creation_time = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),      //设置存档创建时间为当前时间，由于包装属性set设置了用字符串传入(方便存档文件读取)，所以得转成字符串再赋值
                        Name = New_save_name.Text,
                        Jump = 1,
                        Record = new List<string>() { "0-0" }
                    }
                );
                Option.save_choose = (New_save_UUID);
                Option.Wrtie_out();
                Page_frame.Navigate(new Rebillion_qoutes());           //跳转到Rebillion界面
            }

           //恢复按钮响应
           ((Button)sender).IsEnabled = true;

        }



        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            //禁用按钮，避免重复触发
            ((Button)sender).IsEnabled = false;

            //播放并耐心等待按钮的粒子动画完成
            await Button_animation.Animation((Button)sender, Red_colors);

            Page_frame.Go_back();

            //恢复按钮响应
            ((Button)sender).IsEnabled = true;
        }

        #endregion

    }
}
