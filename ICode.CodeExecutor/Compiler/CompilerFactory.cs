namespace ICode.CodeExecutor.Compiler
{
    public class CompilerFactory
    {
        private static ICompiler _cppCompiler;
        public static ICompiler GetInstance(string lang)
        {
            if (lang == "cpp")
            {
                if (_cppCompiler == null)
                    _cppCompiler = new CppCompiler("Cpp", ".cpp");
                return _cppCompiler;
            }
            return null;
        }
    }
}
