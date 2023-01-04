namespace ICode.CodeExecutor.Utils
{
    public class Cleaner
    {
        public static async Task Execute()
        {
            List<Task> tasks = new List<Task>();
            string []files = Directory.GetFiles("Data", "*", SearchOption.AllDirectories);
            foreach (string file in files) 
            {
                if (File.GetCreationTime(file).AddMinutes(1) < DateTime.Now) 
                {
                    tasks.Add(Task.Run(() => File.Delete(file)));
                }
            }
            await Task.WhenAll(tasks);
        }
    }
}
