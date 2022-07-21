using BootstrapBlazor.Components;
using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Services;

namespace Web.Pages
{
    public partial class CreateProblem
    {
        [Inject] private ITagService TagService { get; set; }
        [Inject] private IProblemService ProblemService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public ToastService ToastService { get; set; }
        private string TagsName { get; set; } = "";
        private List<TagDTO> Tags { get; set; }
        private List<SelectedItem> TagSelectItems { get; set; } = new List<SelectedItem>();
        private ProblemInput ProblemInput { get; set; } = new ProblemInput
        {
            TestCases = new List<TestcaseInput>(),
            Tags = new List<string>()
        };
        protected override async Task OnInitializedAsync()
        {
            Tags = await TagService.GetAll();
            Tags.ForEach(x =>
            {
                TagSelectItems.Add(new SelectedItem(x.ID, x.Name));
            });
            TagsName = "";
        }
        private void AddTestcase()
        {
            ProblemInput.TestCases.Add(new TestcaseInput());
        }

        public async Task<Task> HandleSubmit(EditContext context)
        {
            if (true)
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                ProblemInput.ArticleID = user?.FindFirst(x => x.Type == "ID")?.Value;
                ProblemInput.Tags = TagsName.Split(",").ToList();
                var result = await ProblemService.CreateProblem(ProblemInput);
                if (result == HttpStatusCode.Created)
                {
                    await ToastService.Success("Thông báo", "Tạo problem thành công");
                    ProblemInput = new ProblemInput
                    {
                        TestCases = new List<TestcaseInput>(),
                        Tags = new List<string>()
                    };
                }
                else if (result == HttpStatusCode.Unauthorized)
                {
                    await ToastService.Error("Thông báo", "Bạn chưa đăng nhập");
                }
                else
                {
                    await ToastService.Error("Thông báo", "Lỗi server");
                }
            }
            else
            {
                await ToastService.Error("Thông báo", "Vui lòng tạo ít nhất một testcase");
            }
            return Task.CompletedTask;
        }
    }
}
