using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
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

        protected override async Task OnInitializedAsync()
        {
            problems = await problemService.GetAllAsync();
        }
    }
}
