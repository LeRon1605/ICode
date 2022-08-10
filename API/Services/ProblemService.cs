using API.Models.Entity;
using API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProblemService
    {
    }
    public class ProblemService : IProblemService
    {
        private IProblemRepository _problemRepository;

    }
}
