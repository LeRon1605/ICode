using ICode.CodeExecutor.Compiler;
using ICode.CodeExecutor.Models;

namespace ICode.CodeExecutor.Runner;
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
                string runnerResult = await File.ReadAllTextAsync(commandResult.Result.Trim());
                result.Time = GetTime(runnerResult);
                result.Output = GetOutput(runnerResult);
            }
            result.Memory = commandResult.Memory;
            result.Status = commandResult.Status ? ExecutorStatus.Success : ExecutorStatus.RuntimeError;
        }
        return result;
    }

    private float GetTime(string str)
    {
        return float.Parse(str.Trim('\n').Split("|")[1]);
    }

    private string GetOutput(string str)
    {
        return str.TrimEnd('\n').Split("|")[0];
    }
}