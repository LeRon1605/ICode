namespace ICode.CodeExecutor.Utils
{
    public class Cleaner
    {
        public static async Task Execute()
        {
            List<Task> tasks = new List<Task>();
            string []dirs = Directory.GetDirectories("judge", "*", SearchOption.AllDirectories);
            foreach (string dir in dirs) 
            {
                if (Directory.GetCreationTime(dir).AddMinutes(1) < DateTime.Now) 
                {
                    tasks.Add(Task.Run(() => Directory.Delete(dir, true)));
                }
            }
            await Task.WhenAll(tasks);
        }
    }
}
