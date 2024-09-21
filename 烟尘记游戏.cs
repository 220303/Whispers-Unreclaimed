namespace 烟尘记
{
    public class 烟尘记游戏:IDisposable
    {

        public 烟尘记游戏(Data.Save save)
        {
            //加载存档
            Jump = save.Jump;
            Record = save.Record;                                                  
        }

        public void Dispose()                                                             //game生命周期结束时调用，用于保存游戏进度
        {
            //保存游戏进度到存档列表的对应值
            Data.Save new_save = Data.Saves[Data.Options.Save_choose];                 //其余值不变
            new_save.Jump = Jump;
            new_save.Record = Record;
            Data.Saves[Data.Options.Save_choose]= new_save;
        }

        public int Jump;                                                                   //jump值，剧情跳转索引

        public List<string> Record;                                                        //record数组，储存加载的那个存档的游玩记录（剧情-选项 剧情-选项 剧情-选项 剧情-选项......)

        public string Plot;                                                                //读取的剧情（完整的一段）

        public List<Choice> ChoiceList = new();                               //选项列表，用于储存读取的选项

        public event EventHandler<EventArgs> Story_end;                                    //用于标志游戏进行到结局了



        public void Plot_readin()
        {
            //读入剧情文件
            Plot = File.ReadAllText(Data.Plot_directory_path + Jump + ".txt");
        }



        public void Choices_readin()          //读入选项
        {
            

            //计算有多少个选项文件
            if (Directory.Exists(Data.Choices_directory_path + Jump))                   //检查是否有对应当前剧情的选项目录可用
            {

                int choice_file_number = 0;                                                         //定义choice_file_number 变量用于记录有多少个选项文件

                string[] files = Directory.GetFiles(Data.Choices_directory_path + Jump);                                                     //获取 所有的文件的 包含目录的文件名
                foreach (string file in files)
                {
                    if (System.IO.Path.GetFileName(file).Remove(System.IO.Path.GetFileName(file).Length - 4).All(char.IsDigit))         // 判断这个文件的名字是否是数字，如果是则说明这是一个选项。     或者括号内用 Int32.TryParse(Path.GetFileName(file).Remove(Path.GetFileName(file).Length - 4), out int n)
                    {
                        choice_file_number += 1;
                    }
                }
                //此时的 choice_file_number 变量的值就是文件中有几个选项，但不一定所有文件中的选项都要显示。

                //将需要输出的选项实例化并放入ChoiceList中。
                for (int choice_file_counter = 1; choice_file_counter <= choice_file_number; choice_file_counter++)
                {
                    if (new Choice(Jump, choice_file_counter).check(Record))                       //只给用户他能选择的选项，具体是什么由剧情及选择历史决定
                    {
                        ChoiceList.Add(new Choice(Jump, choice_file_counter));
                    };
                }
            }
            else
            {
                //说明是结局，出发结尾事件
                Story_end(this, new EventArgs());
            }
        }




        public void Choose(int input)                                                        //检测选项 ，此处input为经过WPF后台隐藏代码转换后的用户输入，即用户的选择
        {

            Record.Add(Jump + "-" + input);                          //在用户的历史记录中添加这一次的 剧情-选项

            //这一步的try其实是一道保险，正常不会用到，因为在前台隐藏代码中做过了过滤
            try     //无论用户输入一个多大的数字都进行查找，如果找到了就处理，如果没找到就不处理报错，但继续等待有有意义的输入出现
            {
                Jump = Convert.ToInt16(File.ReadAllText(Data.Choices_directory_path + Jump + "/" + input + "j.txt"));
            }
            catch (System.IO.FileNotFoundException)
            { }

            ChoiceList.Clear();                                               //清空选项List
        }


    }
}
