namespace 烟尘记
{
    public class Choice
    {
        public Choice(int jump, int choice_number)
        {

            content = File.ReadAllText(Data.Choices_directory_path + jump + "/" + choice_number + ".txt");      //把选项内容加载到content 变量里

            if (File.Exists(Data.Choices_directory_path + jump + "/" + choice_number + "condition.txt"))
            {
                string choice_condition_string = File.ReadAllText(Data.Choices_directory_path + jump + "/" + choice_number + "condition.txt");
                condition = new List<string>(choice_condition_string.Split(' '));             //把 选项输出的条件 加载到condition 列表里
            }
        }

        public string content;
        public List<string> condition;

        public bool check(List<string> check_record)
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
}
