using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly ISubmissionService _submissionService;
        public SubmissionController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        [Authorize]
        public async Task<IActionResult> Submit(string id, SubmissionInput submission)
        {
            SubmissionResult result = await _submissionService.Submit(id, submission);
            return RedirectToAction("Detail", new { id = result.ID });
        }

        [Authorize]        
        public async Task<IActionResult> Detail(string id)
        {
            ServiceResponse<SubmissionResponse> result = await _submissionService.GetById(id);
            if (result.State)
            {
                List<SubmissionDTO> userSubmissions = await _submissionService.GetSubmissionOfProblem(result.Data.Submission.Problem.ID, User.FindFirstValue(ClaimTypes.NameIdentifier));

                ViewBag.userSubmissions = userSubmissions;
                return View(result.Data);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
