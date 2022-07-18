using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Services;

namespace Web.Pages
{
    public partial class ProblemDetail
    {
        [Parameter]
        public string ProblemID { get; set; }
        [Inject]
        private IProblemService problemService { get; set; }
        public ProblemDTO Problem { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Problem = await problemService.GetByIDAsync(ProblemID);
        }
    }
}
