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
            Data.Save new_save = Data.Saves[Data.Options.Save_choose-1];                 //其余值不变
            new_save.Jump = Jump;
            new_save.Record = Record;
            Data.Saves[Data.Options.Save_choose-1]= new_save;
        }



        public int Jump;                                                                   //jump值，剧情跳转索引

        public List<string> Record;                                                        //record数组，储存加载的那个存档的游玩记录（剧情-选项 剧情-选项 剧情-选项 剧情-选项......)



        public void Choose(int input)                                                        //检测选项 ，此处input为经过WPF后台隐藏代码转换后的用户输入，即用户的选择
        {
            Record.Add(Jump + "-" + input);                                              //在用户的历史记录中添加这一次的 剧情-选项
            Jump = Data.Nodes[Jump - 1].Choices[input - 1].Jump;                         //利用选项编号改变Jump的值
        }

    }
}
