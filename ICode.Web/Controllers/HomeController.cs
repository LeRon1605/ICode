using CodeStudy.Models;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Statistic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly IUserService _userService;
        public HomeController(IProblemService problemService, IUserService userService)
        {
            _problemService = problemService;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            List<ProblemDTO> problems = await _problemService.GetAll();
            List<ProblemDTO> newProblems = await _problemService.GetNewProblems();
            List<ProblemStatistic> hotProblems = await _problemService.GetHotProblems();
            List<UserRank> rank = await _userService.GetUserRank();

            ViewBag.problems = problems;
            ViewBag.newProblems = newProblems;
            ViewBag.hotProblems = hotProblems;
            ViewBag.rank = rank;
            return View();
        }

        [Authorize]
        public IActionResult Home()
        {
            return Content(User.Identity.Name);
        }
    }
}
