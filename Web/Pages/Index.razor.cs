using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Web.Services;

namespace Web.Pages
{
    public partial class Index
    {
        [Inject]
        private IProblemService problemService { get; set; }
        private List<ProblemDTO> problems { get; set; }
        private List<ProblemDTO> problems_display { get; set; }
        private string keyword { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            problems = await problemService.GetAllAsync();
            problems_display = problems;
        }

        private void HandleSubmit(EditContext context)
        {
            problems_display = null;
            problems_display = problems.Where(x => x.Name.Contains(keyword)).ToList();
        }
    }
}
