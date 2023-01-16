using ICode.CodeExecutor.Runner.Commands;

namespace ICode.CodeExecutor.Compiler;
public class CompilerFactory
{
    private static ICompiler _cppCompiler;
    private static ICompiler _cCompiler;
    private static ICompiler _javaCompiler;
    public static ICompiler GetInstance(string lang)
    {
        if (lang == "cpp")
        {
            if (_cppCompiler == null)
                _cppCompiler = new Compiler(new CppCommand("main.cpp", "main.exe"));
            return _cppCompiler;
        }
        else if (lang == "c")
        {
            if (_cppCompiler == null)
                _cCompiler = new Compiler(new CCommand("main.c", "main.exe"));
            return _cCompiler;
        }
        else if (lang == "java")
        {
            if (_javaCompiler == null)
                _javaCompiler = new Compiler(new JavaCommand("Main.java", "Main"));
            return _javaCompiler;
        }
        return null;
    }
}