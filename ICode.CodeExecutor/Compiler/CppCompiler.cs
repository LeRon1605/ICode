using ICode.CodeExecutor.Models;
using ICode.CodeExecutor.Utils;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ICode.CodeExecutor.Compiler
{
    public class CppCompiler : BaseCompiler
    {
        public CppCompiler(string language, string extension) : base(language, extension)
        {
        }

        public override async Task<CompileResult> Compile(string code)
        {
            string id = UniqueID.GenerateID(code);

            if (File.Exists(GenerateOuputPath(id)))
            {
                if (File.Exists(GenerateErrorPath(id)))
                {
                    return new CompileResult 
                    {
                        Result = await File.ReadAllTextAsync(GenerateErrorPath(id)),
                        Status = false
                    };
                }
            }
            else
            {
                await File.WriteAllTextAsync(GenerateCodePath(id), code);
                CommandResult commandResult = Command.Execute($"g++ {GenerateCodePath(id)} -o {GenerateOuputPath(id)} 2>&1");
                if (!commandResult.Status)
                {
                    await File.WriteAllTextAsync(GenerateErrorPath(id), commandResult.Result);
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

        public override async Task<CommandResult> Execute(string id, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Command.Execute($"timeout 10s /usr/bin/time -f '|%E' {GenerateOuputPath(id)} 2>&1");
            }
            else
            {
                await File.WriteAllTextAsync(GenerateInputPath(id), input);
                return Command.Execute($"timeout 10s /usr/bin/time -f '|%E' {GenerateOuputPath(id)} < {GenerateInputPath(id)} 2>&1");
            }
        }
    }
}
