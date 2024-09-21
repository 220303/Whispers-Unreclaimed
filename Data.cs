using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static 烟尘记.Data;

namespace 烟尘记
{
    public static class Data
    {

        public static string Saves_directory_path = "saves/";
        public static string Options_file_path = "Options.txt";
        public static string Choices_directory_path = "choices/";
        public static string Plot_directory_path = "plots/";
        public static string Rebillion_qoutes_file_path = "Rebillion Qoutes.txt";


        public static string[] Rebillion_qoutes = null;

        public static void Rebillion_qoutes_read_in()
        {
            string qoutescontent = File.ReadAllText(Rebillion_qoutes_file_path);
            Rebillion_qoutes = qoutescontent.Split('\n');
        }

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
            foreach(Save save in Saves)
            {
                string[] content_array = new string[2];                                                  //创建数组content_array

                content_array[0] = Convert.ToString(save.Jump);
                content_array[1] = string.Join(' ', save.Record);

                string new_save_content = string.Join('\n', content_array);                              //将数据重新组合成一个字符串

                File.WriteAllText(Saves_directory_path + save.Name + ".txt", new_save_content);          //覆写掉存档内原有内容
            }
        }








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
