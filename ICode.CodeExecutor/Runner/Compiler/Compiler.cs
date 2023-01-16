using ICode.CodeExecutor.Models;
using ICode.CodeExecutor.Runner.Commands;
using ICode.CodeExecutor.Utils;

namespace ICode.CodeExecutor.Compiler;
public class Compiler : ICompiler
{
    private readonly ICommand _command;
    public Compiler(ICommand command)
    {
        _command = command;
    }
    public async Task<CompileResult> Compile(string code)
    {
        string id = UniqueID.GenerateID(code);

        if (File.Exists(_command.GenerateExecPath(id)))
        {
            if (File.Exists(_command.GenerateErrorPath(id)))
            {
                return new CompileResult
                {
                    Result = await File.ReadAllTextAsync(_command.GenerateErrorPath(id)),
                    Status = false
                };
            }
        }
        else
        {
            if (!Directory.Exists(Path.GetFullPath(_command.GenerateDir(id))))
            {
                Directory.CreateDirectory(Path.GetFullPath(_command.GenerateDir(id)));
            }
            await File.WriteAllTextAsync(_command.GenerateCodePath(id), code);
            CommandResult commandResult = Command.Execute(_command.GetCompileCommand(id));
            if (!commandResult.Status)
            {
                await File.WriteAllTextAsync(_command.GenerateErrorPath(id), commandResult.Result);
                return new CompileResult
                {
                    Result = commandResult.Result,
                    Status = false
                };
            }
        }
        return new CompileResult
        {
            Result = id,
            Status = true
        };
    }

    public async Task<CommandResult> Execute(string id, string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Command.Execute($"timeout 30s /usr/bin/time -f '|%S' {_command.GetExecuteCommand(id)}");
        }
        else
        {
            await File.WriteAllTextAsync(_command.GenerateInputPath(id), input);
            return Command.Execute($"timeout 30s /usr/bin/time -f '|%S' {_command.GetExecuteCommand(id, true)}");
        }
    }
}