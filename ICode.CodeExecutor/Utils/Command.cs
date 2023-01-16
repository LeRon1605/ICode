using System.Diagnostics;
using ICode.CodeExecutor.Models;

namespace ICode.CodeExecutor.Utils
{
    public class Command
    {
        public static CommandResult Execute(string command)
        {
            try
            {
                long memory = 0;
                ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash", $"-c \"{command}\"");
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                do
                {
                    if (!proc.HasExited)
                    {
                        proc.Refresh();
                        memory = proc.PeakWorkingSet64;
                    }
                } while (!proc.WaitForExit(500));
                return new CommandResult
                {
                    Result = proc.StandardOutput.ReadToEnd(),
                    Status = (proc.ExitCode == 0),
                    Memory = (int)(memory * 1e-3)
                };
            }
            catch (Exception objException)
            {
                Console.WriteLine("Execute Command failed" + objException.Message);
                return null;
            }
        }
    }
}