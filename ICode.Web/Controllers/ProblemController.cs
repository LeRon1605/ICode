using CodeStudy.Models;
using ICode.Models.Comment;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class ProblemController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ITagService _tagService;
        private readonly ISubmissionService _submissionService;
        private readonly ICommentService _commentService;

        public ProblemController(IProblemService problemService, ITagService tagService, ISubmissionService submissionService, ICommentService commentService)
        {
            _problemService = problemService;
            _tagService = tagService;
            _submissionService = submissionService;
            _commentService = commentService;
        }

        public async Task<IActionResult> Index(string id)
        {
            ProblemDTO problem = await _problemService.GetById(id);

            if (problem == null)
                return RedirectToAction("Index", "Home");

            List<CommentDetail> comments = await _commentService.GetCommentsOfProblem(id);
            ViewBag.comments = comments;

            return View(problem);
        }

        public async Task<IActionResult> Search(string tag = "", string sort = "", string keyword = "", string orderBy = "asc", string level = "", int page = 1)
        {
            PagingList<ProblemDTO> problems = await _problemService.GetPage(page, 12, keyword, tag, null, level, sort, orderBy);
            List<TagDTO> tags = await _tagService.GetAll();

            ViewBag.tags = tags;
            ViewBag.problems = problems;

            ViewData["tag"] = tag;
            ViewData["level"] = level;
            ViewData["sort"] = sort;
            ViewData["keyword"] = keyword;
            ViewData["page"] = page;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Submissions(string id, int page = 1, bool your = false, string keyword = "", string language = "", bool? status = null, DateTime? date = null, string sort = "", string orderBy = "")
        {
            ProblemDTO problem = await _problemService.GetById(id);
            if (problem == null)
            {
                return View();
            }
            else
            {
                PagingList<SubmissionDTO> submissions;

                if (your)
                {
                   submissions = await _submissionService.GetPageSubmissionsOfProblem(id, page, 10, User.FindFirstValue(ClaimTypes.NameIdentifier), language, status, date, sort, orderBy);
                }
                else
                {
                    submissions = await _submissionService.GetPageSubmissionsOfProblem(id, page, 10, keyword, language, status, date, sort, orderBy);
                }

                ViewBag.submissions = submissions;

                ViewData["page"] = page;
                ViewData["keyword"] = keyword;
                ViewData["language"] = language;
                ViewData["status"] = status;
                ViewData["your"] = your;
                ViewData["date"] = date;
                ViewData["sort"] = sort;
                return View(problem);
            }
        }
    }
}
