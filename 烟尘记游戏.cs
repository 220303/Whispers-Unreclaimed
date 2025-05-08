namespace 烟尘记
{
    public class 烟尘记游戏
    {

        //存放Plots的位置
        private string Plot_directory_path = Data.Filesystem_directory_path + "plots/";
        //存放Choices的位置
        private string Choices_directory_path = Data.Filesystem_directory_path + "choices/";


        //用于储存剧情和选项的结构体
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
            public string Plot;
            public List<Choice> Choices;

            public Node(string plot, List<Choice> choices)
            {
                this.Plot = plot;
                this.Choices = choices;
            }
        }




        public 烟尘记游戏(Save.save save)                                                     //初始化游戏逻辑对象（利用所选择的存档内容)
        {
            //读取剧情 (由于玩游戏会损坏Nodes中Choice的condition，所以无论曾经是否读取过，都重新读取一遍)
            Nodes_read_in();
            //加载存档
            Jump = save.Jump;
            Record = save.Record;
        }

        public void Save_game()                                                             //保存游戏进度
        {
            //保存游戏进度到存档列表的对应值
            Save.save new_save = Save.saves[Option.save_choose-1];                 //其余值不变
            new_save.Jump = Jump;
            new_save.Record = Record;
            Save.saves[Option.save_choose-1]= new_save;

            //立即写入存档文件
            Save.Write_out();
        }



        public List<Node> Nodes = new List<Node>();                                        //储存所有剧情的Nodes
        public int Jump;                                                                   //jump值，剧情跳转索引
        public List<string> Record;                                                        //record数组，储存加载的那个存档的游玩记录（剧情-选项 剧情-选项 剧情-选项 剧情-选项......)



        //做出选择
        public void Choose(int input)                                                      //此处input为经过WPF后台隐藏代码转换后的用户输入，即用户的选择
        {
            Record.Add(Jump + "-" + input);                                              //在用户的历史记录中添加这一次的 剧情-选项
            Jump = Nodes[Jump - 1].Choices[input - 1].Jump;                         //利用选项编号改变Jump的值
        }


        //读取所有剧情
        public void Nodes_read_in()
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
    }
}
