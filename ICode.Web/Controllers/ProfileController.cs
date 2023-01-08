using CodeStudy.Models;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            UserDTO me = await _userService.GetProfile();
            if (me == null)
            {
                throw new Exception();
            }
            List<ProblemDTO> problemsSolved = await _userService.GetProblemSolvedByUser();

            ViewBag.problemsSolved = problemsSolved;
            return View(me);
        }
    }
}
