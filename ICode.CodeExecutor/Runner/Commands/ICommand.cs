namespace ICode.CodeExecutor.Runner.Commands;

public abstract class ICommand
{
    protected readonly string codeFile;
    protected readonly string executeFile;
    public ICommand(string codeFile, string executeFile)
    {
        this.codeFile = codeFile;
        this.executeFile = executeFile;
    }

    public string GenerateDir(string id) => $"judge/{id}";
    public string GenerateCodePath(string id) => $"{GenerateDir(id)}/{codeFile}";
    public string GenerateExecPath(string id) => $"{GenerateDir(id)}/{executeFile}";
    public string GenerateOutputPath(string id) => $"{GenerateDir(id)}/output.txt";
    public string GenerateErrorPath(string id) => $"{GenerateDir(id)}/error.txt";
    public string GenerateInputPath(string id) => $"{GenerateDir(id)}/input.txt";

    public abstract string GetCompileCommand(string id);
    public abstract string GetExecuteCommand(string id, bool hasInput = false);
}