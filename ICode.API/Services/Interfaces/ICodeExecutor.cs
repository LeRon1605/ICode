using Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICodeExecutor
    {
        Task<ExecutorResult> ExecuteCode(string code, string language, string input);
    }
}
