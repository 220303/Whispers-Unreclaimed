namespace 烟尘记
{
    public static class Data
    {

        //游戏所有文件的根目录
        public static string Filesystem_directory_path = "../Filesystem/";

        //存放背景图片的位置
        public static string Image_directory_path = Filesystem_directory_path + "Resource/image/";







        //为MainWindow生成的一个全局对象，使得在其他Page中也能访问到MainWindow中的内容，比如通过MainWindow.Page_frame来切换页面来切换页面
        public static MainWindow Main_window;






        //存放背景音乐的位置
        public static string Music_directory_path = Filesystem_directory_path + "Resource/music/";

        public struct Global_music_struct
        {
            //保存 Global_media_element
            public MediaElement Global_media_element;
            //当前页面所属音乐播放组的记录
            public string Current_music_page_group;
        }

        //全局唯一静态音乐播放器
        public static Global_music_struct Global_music;


        public static void Global_music_start(string Current_music_page_group)
        {

            //初始化全局音频播放器
            Data.Global_music.Global_media_element = Data.Main_window.global_media_element;

            //设置开始时的音频组标识
            Data.Global_music.Current_music_page_group = Current_music_page_group;

            //设置音频播放器播放游戏背景音乐
            Data.Global_music.Global_media_element.Source = new Uri(Data.Music_directory_path + "程序背景音.mp3", UriKind.RelativeOrAbsolute);

            //设置音量
            Data.Global_music.Global_media_element.Volume = Data.Option.Music_volume;

            //音频播放器开始播放
            Data.Global_music.Global_media_element.Play();

            //设置音频循环播放
            Data.Global_music.Global_media_element.MediaEnded += (s, e) =>
            {
                Data.Global_music.Global_media_element.Position = TimeSpan.Zero;
                Data.Global_music.Global_media_element.Play();
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

                    if ((new_music_page_group != Data.Global_music.Current_music_page_group) && (new_music_page_group != "all"))
                    {
                        // 停止当前音频
                        Data.Global_music.Global_media_element.Stop();
                        // 根据新的音频组标识设置对应的音频
                        switch (new_music_page_group)
                        {
                            case "program":
                                Data.Global_music.Global_media_element.Source = new Uri(Data.Music_directory_path + "程序背景音.mp3", UriKind.RelativeOrAbsolute);
                                break;
                            case "game":
                                Data.Global_music.Global_media_element.Source = new Uri(Data.Music_directory_path + "游戏背景音.wav", UriKind.RelativeOrAbsolute);
                                break;
                        }

                        // 播放新音频
                        Data.Global_music.Global_media_element.Play();
                        //更改当前页面分组的记录
                        Data.Global_music.Current_music_page_group = new_music_page_group;
                    }

                }
            };

            // 启动计时器
            timer.Start();
        }








        //存放Plots的位置
        public static string Plot_directory_path = Filesystem_directory_path + "plots/";
        //存放Choices的位置
        public static string Choices_directory_path = Filesystem_directory_path + "choices/";

        public struct Choice
        {
            public Choice(string content, int jump, List<string> condition)
            {
                this.Content = content;
                this.Jump = jump;
                this.condition = condition;
            }

            public string Content;
            public int Jump;
            public List<string> condition;

            //自带的check方法，接受一个 record，并返回这个record是否满足输出本选项的条件
            public bool Check(List<string> check_record)
            {
                if (condition == null)
                {
                    return true;
                }
                else
                {
                    int require_time = ToInt32(condition[condition.Count - 1].Trim());
                    int time = 0;
                    condition.RemoveAt(condition.Count - 1);
                    foreach (string condition_item in condition)
                    {
                        if (check_record.Exists(record_item => record_item == condition_item))
                        {
                            time += 1;
                        }
                    }
                    return (time >= require_time);
                }
            }
        }

        public struct Node
        {
            public Node(string plot, List<Choice> choices)
            {
                this.Plot = plot;
                this.Choices = choices;
            }

            public string Plot;
            public List<Choice> Choices;
        }

        public static List<Node> Nodes = new List<Node>();

        public static void Nodes_read_in()
        {
            string[] plot_files = Directory.GetFiles(Plot_directory_path);
            var sort_plot_files = plot_files
     .Select(file => new
     {
         FilePath = file,
         FileName = Path.GetFileNameWithoutExtension(file),
         NumericValue = int.Parse(Path.GetFileNameWithoutExtension(file)) // 将文件名转换为数字
     })
    .OrderBy(file => file.NumericValue) // 根据数字排序
    .Select(file => file.FilePath) // 选择原始文件路径
    .ToArray();
            for (int i = 1; i < sort_plot_files.Length; i++)
            {
                if (Directory.Exists(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1])))
                {
                    //准备好这个剧情对应的选项
                    List<Choice> temp_choices = new List<Choice>();
                    int j = 1;
                    while (true)
                    {
                        //如果这个选项存在,就把它创建并加入temp_choices,如果没有condition，那就填null，一旦全部添加完毕立刻结束无限循环
                        if (File.Exists(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + ".txt"))
                        {
                            //创建选项，设置Jump和Content，暂时设置condition为null
                            Choice temp_choice = new Choice(File.ReadAllText(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + ".txt"), ToInt32(File.ReadAllText(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + "j.txt")), null);
                            //检查选项是否有条件，若有则赋值，若无则保持null
                            if (File.Exists(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + "C.txt"))
                            {
                                temp_choice.condition = new List<string>(File.ReadAllText(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + "C.txt").Split(' '));
                            }

                            /*
                            以上创建temp_choice的另一种实现(不需要Choice的构造函数)
                            temp_choice.Content = File.ReadAllText(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + ".txt");
                            temp_choice.Jump = ToInt32(File.ReadAllText(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + "j.txt"));
                            if (File.Exists(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + "C.txt"))
                            {
                                temp_choice.condition = new List<string>(File.ReadAllText(Choices_directory_path + Path.GetFileNameWithoutExtension(sort_plot_files[i - 1]) + "/" + j + "C.txt").Split(' '));
                            }
                            else
                            {
                                temp_choice.condition = null;
                            }
                            */

                            temp_choices.Add(temp_choice);
                        }
                        else
                        {
                            break;
                        }
                        j++;
                    }

                    //把剧情和选项赋值给节点
                    Nodes.Add(new Node(File.ReadAllText(sort_plot_files[i - 1]), temp_choices));
                }
                else  //没有选项，说明是结局
                {
                    Nodes.Add(new Node(File.ReadAllText(sort_plot_files[i - 1]), null));
                }
            }
        }







        //存放Rebillion Qoutes的位置
        public static string Rebillion_qoutes_file_path = Filesystem_directory_path + "Rebillion Qoutes.txt";

        public static string[] Rebillion_qoutes;

        public static void Rebillion_qoutes_read_in()
        {
            string qoutescontent = File.ReadAllText(Rebillion_qoutes_file_path);
            Rebillion_qoutes = qoutescontent.Split('\n');
        }






        //存放存档的位置
        public static string Saves_directory_path = Filesystem_directory_path + "Saves/";

        public struct Save
        {
            public string Name { get; set; }
            public int Jump;
            public List<string> Record;
        }

        public static ObservableCollection<Save> Saves = new();


        public static void Save_read_in()
        {
            string[] save_files = Directory.GetFiles(Saves_directory_path);
            foreach (string file in save_files)
            {
                Save save = new();

                string savecontent = File.ReadAllText(file);                                                                //创建一个字符串变量用于储存从存档中读取的数据
                string[] content_array = null;                                                                              //创建数组content_array
                content_array = savecontent.Split('\n');                                                                    //通过空格分开存档中不同的数据，并存入数组content_array

                save.Name = System.IO.Path.GetFileName(file).Remove(System.IO.Path.GetFileName(file).Length - 4);           //存档名即为存档文件名(去掉后缀)
                save.Jump = ToInt16(content_array[0]);                                                                      //Jump
                save.Record = new List<string>(content_array[1].Split(' '));                                                //Record

                Saves.Add(save);
            }
        }


        //游戏结束后，把新的进度写入存档
        public static void Save_write_out()
        {
            foreach (Save save in Saves)
            {
                string[] content_array = new string[2];                                                  //创建数组content_array

                content_array[0] = Convert.ToString(save.Jump);
                content_array[1] = string.Join(' ', save.Record);

                string new_save_content = string.Join('\n', content_array);                              //将数据重新组合成一个字符串

                File.WriteAllText(Saves_directory_path + save.Name + ".txt", new_save_content);          //覆写掉存档内原有内容
            }
        }







        //存放Options的位置
        public static string Options_file_path = Filesystem_directory_path + "Options.txt";

        public struct Option_struct
        {
            public int Save_choose { get; set; }           //注意，从1开始计数，不是从0开始
            public int Plot_font_size { get; set; }
            public int Plot_print_speed { get; set; }
            public double Music_volume { get; set; }

        }

        public static Option_struct Option = new Option_struct();

        public static void Option_read_in()
        {
            string savecontent = File.ReadAllText(Options_file_path);                               //从设置文件中读取所有内容

            string[] content_array = null;
            content_array = savecontent.Split('\n');
            Data.Option.Save_choose = ToInt16(content_array[0]);
            Data.Option.Plot_font_size = ToInt16(content_array[1]);
            Data.Option.Plot_print_speed = ToInt16(content_array[2]);
            Data.Option.Music_volume = ToDouble(content_array[3]);
        }

        public static void Option_wrtie_out()
        {
            string[] content_array = new string[4];
            content_array[0] = Convert.ToString(Data.Option.Save_choose);
            content_array[1] = Convert.ToString(Data.Option.Plot_font_size);
            content_array[2] = Convert.ToString(Data.Option.Plot_print_speed);
            content_array[3] = Convert.ToString(Data.Option.Music_volume);

            string new_options_content = string.Join('\n', content_array);                        //将数据重新组合成一个字符串

            File.WriteAllText(Options_file_path, new_options_content);                            //覆写掉设置文件内原有内容
        }


    }
}
