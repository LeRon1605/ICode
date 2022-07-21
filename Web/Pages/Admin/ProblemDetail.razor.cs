using BootstrapBlazor.Components;
using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Services;

namespace Web.Pages.Admin
{
    public partial class ProblemDetail
    {
        [Inject] private ITagService TagService { get; set; }
        [Inject] private IProblemService ProblemService { get; set; }
        private string TagsName { get; set; } = "";
        private List<TagDTO> Tags { get; set; }
        private List<SelectedItem> TagSelectItems { get; set; } = new List<SelectedItem>();
        private ProblemDTO problems { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Tags = await TagService.GetAll();
            Tags.ForEach(x =>
            {
                TagSelectItems.Add(new SelectedItem(x.ID, x.Name));
            });
            TagsName = "";
        }
    }
}
