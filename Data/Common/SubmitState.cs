using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Common
{
    public enum SubmitState
    {
        Success,
        TimeLimit,
        MemoryLimit,
        WrongAnswer,
        RuntimeError,
        CompilerError,
        Pending
    }
}
