using System;
using System.Collections.Generic;
using System.Text;

namespace ICode.Models
{
    public enum ServiceState
    {
        Success,
        ValidationError, 
        EntityNotFound,
        InvalidAction
    }
}
