namespace ICode.CodeExecutor.Runner.Commands;
public class CppCommand : ICommand
{
    public CppCommand(string codeFile, string executeFile) : base(codeFile, executeFile)
    {
    }

    public override string GetCompileCommand(string id)
    {
        return $"g++ {GenerateCodePath(id)} -o {GenerateExecPath(id)} 2>&1";
    }

    public override string GetExecuteCommand(string id, bool hasInput)
    {
        if (hasInput)
        {
            return $"{GenerateExecPath(id)} < {GenerateInputPath(id)} &> {GenerateOutputPath(id)} && echo {GenerateOutputPath(id)}";
        }
        return $"{GenerateExecPath(id)} &> {GenerateOutputPath(id)} && echo {GenerateOutputPath(id)}";
    }
}
