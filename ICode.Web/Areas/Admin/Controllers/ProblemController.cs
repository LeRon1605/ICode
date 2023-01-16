using CodeStudy.Models;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProblemController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ITagService _tagService;
        public ProblemController(IProblemService problemService, ITagService tagService)
        {
            _problemService = problemService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index(string tag = "", string sort = "", string keyword = "", string orderBy = "asc", string level = "", int page = 1)
        {
            PagingList<ProblemDTO> problems = await _problemService.GetPage(page, 20, keyword, tag, null, level, sort, orderBy);
            List<TagDTO> tags = await _tagService.GetAll();

            ViewBag.tags = tags;
            ViewBag.problems = problems;

            ViewData["tag"] = tag;
            ViewData["sort"] = sort;
            ViewData["level"] = level;
            ViewData["keyword"] = keyword;
            ViewData["page"] = page;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Insert()
        {
            List<TagDTO> tags = await _tagService.GetAll();

            ViewBag.tags = tags;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Insert(ProblemInput data)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Insert", "Problem", new { area = "Admin" });
            }

            bool result = await _problemService.Add(data);
            if (result)
            {
                TempData["success"] = "Thêm bài tập thành công";
            }
            else
            {
                TempData["error"] = "Thêm bài tập không thành công.";
            }
            return RedirectToAction("Insert", "Problem", new { area = "Admin" });
        }
    }
}
