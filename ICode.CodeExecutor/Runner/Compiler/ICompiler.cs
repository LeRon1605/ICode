using ICode.CodeExecutor.Models;

namespace ICode.CodeExecutor.Compiler;
public interface ICompiler
{
    Task<CompileResult> Compile(string code);
    Task<CommandResult> Execute(string id, string input);
}