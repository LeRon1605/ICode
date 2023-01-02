using ICode.CodeExecutor.Models;

namespace ICode.CodeExecutor.Compiler
{
    public abstract class BaseCompiler: ICompiler
    {
        public string Language { get; set; }
        public string Extension { get; set; }
        public BaseCompiler(string language, string extension)
        {
            Language = language;
            Extension = extension;
            string[] folders = new string[] { "Code", "Exec", "Error", "Input", "Output" };
            foreach (string folder in folders)
            {
                if (!Directory.Exists(Path.GetFullPath($"Data/{Language}/{folder}")))
                {
                    Directory.CreateDirectory(Path.GetFullPath($"Data/{Language}/{folder}"));
                }
            }
        }

        public string GenerateCodePath(string id) => $"Data/{Language}/Code/{id}{Extension}";
        public string GenerateExecPath(string id) => $"Data/{Language}/Exec/{id}";
        public string GenerateOutputPath(string id, string input = "") => $"Data/{Language}/Output/{id}_{input}.txt";
        public string GenerateErrorPath(string id) => $"Data/{Language}/Error/{id}.txt";
        public string GenerateInputPath(string id) => $"Data/{Language}/Input/{id}.txt";

        public abstract Task<CompileResult> Compile(string code);
        public abstract Task<CommandResult> Execute(string id, string input);
    }
}
