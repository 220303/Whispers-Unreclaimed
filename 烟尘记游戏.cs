using System.Collections.Generic;
using static 烟尘记.烟尘记游戏;

namespace 烟尘记
{
    public class 烟尘记游戏
    {

        public string Name;

        //存放故事的目录
        private string Story_directory_path = Data.Filesystem_directory_path + "Story/";


        public List<Node> Nodes = new List<Node>();                                        //储存所有剧情的Nodes
        public int Jump;                                                                   //jump值，剧情跳转索引
        public List<string> Record;                                                        //record数组，储存加载的那个存档的游玩记录（剧情-选项 剧情-选项 剧情-选项 剧情-选项......)

        public Text_style default_text_style = new Text_style
        {
            FontFamily = new FontFamily("Microsoft YaHei"),
            FontSize = Option.plot_font_size,
            Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
            Bold = false,
            Italic = false,
            Underline = false,
            Strikethrough = false
        };

        public 烟尘记游戏(Save.save save)                                                     //初始化游戏逻辑对象（利用所选择的存档内容)
        {
            //读取剧情 (由于玩游戏会损坏Nodes中Choice的condition，所以无论曾经是否读取过，都重新读取一遍)
            Nodes_read_in();

            //加载存档
            Name = save.Name;
            Jump = save.Jump;
            Record = save.Record;
        }


        public void Save_game()                                                             //保存游戏进度
        {
            //保存游戏进度到存档列表的对应值
            Save.save new_save = Save.saves[Option.save_choose - 1];                 //除record和jump外，其余值不变
            new_save.Jump = Jump;
            new_save.Record = Record;
            Save.saves[Option.save_choose - 1] = new_save;

            //立即写入存档文件
            Save.Write_out();
        }


        //做出选择
        public void Choose(int input)                                                      //此处input为经过WPF后台隐藏代码转换后的用户输入，即用户的选择
        {
            Record.Add(Jump + "-" + input);                                              //在用户的历史记录中添加这一次的 剧情-选项
            Jump = Nodes[Jump - 1].Choices[input - 1].Jump;                              //利用选项编号改变Jump的值，注意此时Nodes里面只剩下了刚才输出的选项，不能输出的已经被删掉了，所以可以直接用Input索引选择的那个choice
        }



        #region 剧情节点相关的结构体和方法


        public struct Choice
        {
            public Choice(int jump, List<string> condition, string content)
            {
                this.Jump = jump;
                this.condition = condition;
                this.Content = content;
            }


            public int Jump;
            public List<string> condition;
            public string Content;

            //自带的check方法，接受一个 record，并返回这个record是否满足输出本选项的条件
            public bool Check(List<string> check_record)
            {
                if (condition == null)
                {
                    return true;
                }
                else
                {
                    int require_time = ToInt32(condition[condition.Count - 1]);
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
            public Node(List<Text_fragment> plot, List<Choice> choices)
            {
                this.Plot = plot;
                this.Choices = choices;
            }

            public List<Text_fragment> Plot;
            public List<Choice> Choices;
        }

        public struct Text_style
        {
            public FontFamily FontFamily { get; set; }
            public double FontSize { get; set; }
            public Brush Foreground { get; set; }
            public bool Bold { get; set; }
            public bool Italic { get; set; }
            public bool Underline { get; set; }
            public bool Strikethrough { get; set; }
        }

        public struct Text_fragment
        {
            public Text_style Style { get; set; }
            public string Text { get; set; }
        }


        public void Nodes_read_in()
        {
            //对于每个节点（遍历整个Story文件夹中的每个子文件夹，不会管子文件，i代表节点文件名的数字，从1开始。）
            for (int i = 1; i <= Directory.GetDirectories(Story_directory_path).Length; i++)
            //解析节点
            {

                //获取当前节点的相关文件路径
                string node_directory_path = Story_directory_path + i + "/";                        //节点目录路径
                string plot_file_path = node_directory_path + "Plot.txt";                           //剧情文件路径
                string choices_file_path = node_directory_path + "Choices/";                        //选项目录路径

                //创建一个新的节点
                Node new_node = new Node(new List<Text_fragment>(), new List<Choice>());

                #region 解析剧情

                //读取剧情文件的所有内容，按行分割成数组
                string[] plot_all_text_array = File.ReadAllText(plot_file_path).Split("\n");

                //去除各项末尾可能的\r
                for (int m = 0; m < plot_all_text_array.Length; m++)
                {
                    plot_all_text_array[m] = plot_all_text_array[m].Trim(); //去除每行的前后可能的\r
                }

                //先按两行一组读取文件，并合并成一个Text_fragment对象，并处理换行符转换。
                for (int j = 0; j < plot_all_text_array.Length; j += 2)
                {
                    //创建风格对象
                    Text_style new_style;

                    //解析风格并存储到刚才的对象中
                    if (plot_all_text_array[j] == "")
                    //如果是空行，说明是默认风格
                    {
                        new_style = default_text_style;
                    }
                    else
                    //如果有自定义风格，则把plot_all_text_array[j]按空格分割并按规则解析为Style('~'代表不采用或默认,'#'代表采用)
                    {
                        string[] style_array = plot_all_text_array[j].Split(' ');
                        new_style = new Text_style
                        {
                            FontFamily = style_array[0] == "~" ? default_text_style.FontFamily : new FontFamily(style_array[0]),
                            FontSize = style_array[1] == "~" ? default_text_style.FontSize : ToDouble(style_array[1]) * default_text_style.FontSize,
                            Foreground = style_array[2] == "~" ? default_text_style.Foreground : new SolidColorBrush(Color.FromArgb(ToByte(style_array[2].Split(',')[0]), ToByte(style_array[2].Split(',')[1]), ToByte(style_array[2].Split(',')[2]), ToByte(style_array[2].Split(',')[3]))),
                            Bold = style_array[3] == "#" ? true : false,
                            Italic = style_array[4] == "#" ? true : false,
                            Underline = style_array[5] == "#" ? true : false,
                            Strikethrough = style_array[6] == "#" ? true : false
                        };
                    }

                    //冻结Brush来避免在多线程环境下不能跨线程传递对象的异常
                    new_style.Foreground.Freeze();

                    //解析内容，添加Text_fragment
                    new_node.Plot.Add(new Text_fragment
                    {
                        //设置Style为刚才解析的new_style
                        Style = new_style,
                        //解析Text，如果是空行，则解释为换行
                        Text = plot_all_text_array[j + 1] == "" ? "\n" : plot_all_text_array[j + 1]
                    });
                }

                //将相邻的风格相同的Text_fragment合并为一个Text_fragment
                int k = 0;
                while (true)
                {
                    if (k >= new_node.Plot.Count - 1)
                    //如果已经到达最后一个Text_fragment，则退出循环
                    {
                        break;
                    }

                    //检查下一个Text_fragment的Style和这一个Text_fragment的Style是否相同
                    if (new_node.Plot[k].Style.Equals(new_node.Plot[k + 1].Style))
                    //如果相同，则合并这两个Text_fragment，并删除下一个Text_fragment。
                    {
                        //合并文本，由于Text_fragment是值类型(结构体)，所以必须采取中间变量来中转。
                        var temp = new_node.Plot[k];
                        temp.Text += new_node.Plot[k + 1].Text;
                        new_node.Plot[k] = temp;

                        //删除被合并的Text_fragment
                        new_node.Plot.RemoveAt(k + 1);

                        //重新开始本次循环，检查下一个相邻的Text_fragment是否还需要合并
                        continue;
                    }
                    else
                    //如果不相同，则继续下一个Text_fragment
                    {
                        k++;
                    }
                }

                #endregion

                #region 解析选项
                if (Directory.Exists(choices_file_path))
                //如果选项目录存在，说明不是结局，则解析选项
                {
                    //对于每个选项（遍历整个Choices文件夹）
                    for (int j = 1; j <= Directory.GetFiles(choices_file_path).Length; j++)
                    //解析选项文本文件
                    {
                        //从选项文件中读取所有内容并按行分割                          
                        string[] choice_array = File.ReadAllText(choices_file_path + ToInt16(j) + ".txt").Split("\n");

                        //去除各项末尾可能的\r
                        for (int m = 0; m < choice_array.Length; m++)
                        {
                            choice_array[m] = choice_array[m].Trim(); //去除每行的前后可能的\r
                        }

                        if (choice_array[1] == "")
                        //如果condition行是空行，说明没有条件，则将选项添加到节点中：Jump是从选项文件的第一行读取的数字，condition为null，内容是从第二行开始的所有内容。
                        {
                            new_node.Choices.Add(new Choice(ToInt16(choice_array[0]), null, string.Join("\n", choice_array.Skip(2))));
                        }
                        else
                        //如果condition行不是空行，说明有条件，则将选项添加到节点中：Jump是从选项文件的第一行读取的数字，condition是从第二行读取的列表，内容是从第三行开始的所有内容。
                        {

                            new_node.Choices.Add(new Choice(ToInt16(choice_array[0]), new List<string>(choice_array[1].Split(' ')), string.Join("\n", choice_array.Skip(2))));
                        }
                    }
                }
                else
                //如果选项目录不存在，说明是结局，则不解析选项，直接赋值null
                {
                    new_node.Choices = null;
                }

                #endregion

                //添加解析好的新节点
                Nodes.Add(new_node);

            }
        }


        #endregion

    }
}
