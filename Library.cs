namespace 烟尘记
{
    public static class Data
    {
        //游戏所有文件的根目录
        public static string Filesystem_directory_path = "./Filesystem/";

        //存放背景图片的位置
        public static string Image_directory_path = Filesystem_directory_path + "Resource/image/";


        //为MainWindow生成的一个全局对象，使得在其他Page中也能访问到MainWindow中的内容，比如通过MainWindow.Page_frame来切换页面来切换页面
        public static MainWindow Main_window;
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
            Data.Main_window.Page_frame.Navigated += Change_music;
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

                    String new_music_page_group = ((Page)e.Content).Tag?.ToString();                  // 从页面Tag获取音频组标识

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
                                music_player.Source = new Uri(Music_directory_path + "游戏背景音.wav", UriKind.RelativeOrAbsolute);
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
                string[] content_array = null;                                                                              //创建数组content_array
                content_array = savecontent.Split('\n');                                                                    //通过空格分开存档中不同的数据，并存入数组content_array

                save.Name = System.IO.Path.GetFileName(file).Remove(System.IO.Path.GetFileName(file).Length - 4);           //存档名即为存档文件名(去掉后缀)
                save.Jump = ToInt16(content_array[0]);                                                                      //Jump
                save.Record = new List<string>(content_array[1].Split(' '));                                                //Record

                saves.Add(save);
            }
        }

        public static void Write_out()
        {
            foreach (save save in saves)
            {
                string[] content_array = new string[2];                                                  //创建数组content_array

                content_array[0] = Convert.ToString(save.Jump);
                content_array[1] = string.Join(' ', save.Record);

                string new_save_content = string.Join('\n', content_array);                              //将数据重新组合成一个字符串

                File.WriteAllText(Saves_directory_path + save.Name + ".txt", new_save_content);          //覆写掉存档内原有内容
            }
        }
    }

    public static class Option
    {

        //存放Option的位置
        public static string Options_file_path = Data.Filesystem_directory_path + "Option.txt";

        //Option的内容
        public static int save_choose { get; set; }           //注意，从1开始计数，不是从0开始
        public static int plot_font_size { get; set; }
        public static int plot_print_speed { get; set; }
        public static double music_volume { get; set; }
        public static int start_picture_speed { get; set; }


        //Option的方法
        public static void Read_in()
        {
            string savecontent = File.ReadAllText(Options_file_path);                               //从设置文件中读取所有内容

            string[] content_array = savecontent.Split('\n');

            save_choose = ToInt16(content_array[0]);
            plot_font_size = ToInt16(content_array[1]);
            plot_print_speed = ToInt16(content_array[2]);
            music_volume = ToDouble(content_array[3]);
            start_picture_speed = ToInt16(content_array[4]);
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
            ];

            string new_options_content = string.Join('\n', content_array);                        //将数据重新组合成一个字符串

            File.WriteAllText(Options_file_path, new_options_content);                            //覆写掉设置文件内原有内容
        }
    }
}
