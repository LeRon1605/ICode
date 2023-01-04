using CodeStudy.Models;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class ProblemController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ITagService _tagService;

        public ProblemController(IProblemService problemService, ITagService tagService)
        {
            _problemService = problemService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index(string id)
        {
            ProblemDTO problem = await _problemService.GetById(id);

            if (problem == null)
                return RedirectToAction("Index", "Home");
            return View(problem);
        }

        public async Task<IActionResult> Search(string tag = "", string sort = "", string keyword = "", string orderBy = "asc", int page = 1)
        {
            PagingList<ProblemDTO> problems = await _problemService.GetPage(page, 12, keyword, tag, null, sort, orderBy);
            List<TagDTO> tags = await _tagService.GetAll();

            ViewBag.tags = tags;
            ViewBag.problems = problems;

            ViewData["tag"] = tag;
            ViewData["sort"] = sort;
            ViewData["keyword"] = keyword;
            ViewData["page"] = page;
            return View();
        }
    }
}
