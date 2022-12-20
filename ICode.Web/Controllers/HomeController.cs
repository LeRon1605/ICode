using CodeStudy.Models;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Statistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProblemService _problemService;
        public HomeController(IProblemService problemService)
        {
            _problemService = problemService;
        }
        public async Task<IActionResult> Index()
        {
            List<ProblemDTO> problems = await _problemService.GetAll();
            List<ProblemStatistic> hotProblems = await _problemService.GetHotProblems();

            ViewBag.problems = problems;
            ViewBag.hotProblems = hotProblems;
            return View();
        }

        [Authorize]
        public IActionResult Home()
        {
            return Content(User.Identity.Name);
        }
    }
}
