using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static 烟尘记.Data;

namespace 烟尘记
{
    public static class Data
    {
        public static string Filesystem_directory_path = "../Filesystem/";
        //存放Plots的位置
        public static string Plot_directory_path = Filesystem_directory_path+"plots/";
        //存放Choices的位置
        public static string Choices_directory_path = Filesystem_directory_path + "choices/";

        public struct Choice
        {
            /*
             public Choice(string content,int jump,List<string> condition) 
             {
                 this.content = content;
                 this.jump = jump;
                 this.condition = condition;
             }
            */
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
                    int requir_time = ToInt32(condition[condition.Count - 1]);
                    int time = 0;
                    condition.RemoveAt(condition.Count - 1);
                    foreach (string condition_item in condition)
                    {
                        if (check_record.Exists(record_item => record_item == condition_item))
                        {
                            time += 1;
                        }
                    }
                    return (time >= requir_time);
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
                            Choice temp_choice;
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
        public static string Saves_directory_path = Filesystem_directory_path + "saves/";

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

        public struct Option
        {
            public int Save_choose { get; set; }
            public int Plot_font_size { get; set; }
            public int Plot_print_speed { get; set; }
            public int Music_volume { get; set; }
        }

        public static Option Options;


        public static void Options_read_in()
        {
            string savecontent = File.ReadAllText(Options_file_path);                               //从设置文件中读取所有内容

            string[] content_array = null;
            content_array = savecontent.Split('\n');
            Options.Save_choose = ToInt16(content_array[0]);
            Options.Plot_font_size = ToInt16(content_array[1]);
            Options.Plot_print_speed = ToInt16(content_array[2]);
            Options.Music_volume = ToInt16(content_array[3]);
        }

        public static void Options_wrtie_out()
        {
            string[] content_array = new string[4];
            content_array[0] = Convert.ToString(Options.Save_choose);
            content_array[1] = Convert.ToString(Options.Plot_font_size);
            content_array[2] = Convert.ToString(Options.Plot_print_speed);
            content_array[3] = Convert.ToString(Options.Music_volume);

            string new_options_content = string.Join('\n', content_array);                        //将数据重新组合成一个字符串

            File.WriteAllText(Options_file_path, new_options_content);                            //覆写掉设置文件内原有内容
        }



    }
}
