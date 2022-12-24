using CodeStudy.Models;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class ProblemController : Controller
    {
        private readonly IProblemService _problemService;
        public ProblemController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        public async Task<IActionResult> Index(string id)
        {
            ProblemDTO problem = await _problemService.GetById(id);

            if (problem == null)
                return RedirectToAction("Index", "Home");
            return View(problem);
        }
    }
}
