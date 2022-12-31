using ICode.CodeExecutor.Compiler;
using ICode.CodeExecutor.Models;

namespace ICode.CodeExecutor
{
    public class Runner
    {
        public async Task<ExecutorResult> Execute(string code, string lang, string input)
        {
            ICompiler compiler = CompilerFactory.GetInstance(lang);
            ExecutorResult result = new ExecutorResult
            {
                Input = input,
                Output = string.Empty,
                Time = 0,
                Status = ExecutorStatus.Pending
            };

            CompileResult compileResult = await compiler.Compile(code);
            if (!compileResult.Status)
            {
                result.Output = compileResult.Result;
                result.Status = ExecutorStatus.CompilerError;
            }
            else
            {
                CommandResult commandResult = await compiler.Execute(compileResult.Result, input);
                if (commandResult.Status)
                {
                    result.Time = GetTime(commandResult.Result);
                    result.Output = GetOutput(commandResult.Result);
                }
                result.Memory = commandResult.Memory;
                result.Status = commandResult.Status ? ExecutorStatus.Success : ExecutorStatus.RuntimeError;
            }
            return result;
        }

        private float GetTime(string str)
        {
            string timeStr = str.Trim('\n').Split("|")[1];
            string[] timeArr = timeStr.Split(":");
            return float.Parse(timeArr[0]) * 60 * 1000 + float.Parse(timeArr[1]);
        }

        private string GetOutput(string str)
        {
            return str.TrimEnd('\n').Split("|")[0];
        }
    }
}