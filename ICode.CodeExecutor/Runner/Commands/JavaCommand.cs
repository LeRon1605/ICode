namespace ICode.CodeExecutor.Runner.Commands;
public class JavaCommand : ICommand
{
    public JavaCommand(string codeFile, string executeFile) : base(codeFile, executeFile)
    {
    }

    public override string GetCompileCommand(string id)
    {
        return $"javac {GenerateCodePath(id)} 2>&1";
    }

    public override string GetExecuteCommand(string id, bool hasInput)
    {
        if (hasInput)
        {
            return $"java -cp {GenerateDir(id)} {executeFile} < {GenerateInputPath(id)} &> {GenerateOutputPath(id)} && echo {GenerateOutputPath(id)}";
        }
        return $"java -cp {GenerateDir(id)} {executeFile} &> {GenerateOutputPath(id)} && echo {GenerateOutputPath(id)}";
    }
}