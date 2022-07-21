using BlazorMonaco;
using BootstrapBlazor.Components;
using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Services;

namespace Web.Pages
{
    public partial class ProblemDetail
    {
        [Parameter] public string ProblemID { get; set; }
        [Inject] private IProblemService problemService { get; set; }
        [Inject] private ISubmissionService SubmissionService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] public ToastService ToastService { get; set; }
        private MonacoEditor _editor { get; set; }
        private ProblemDTO Problem { get; set; }
        private IEnumerable<SelectedItem> Language { get; set; } = new []
        {
            new SelectedItem ("cpp", "C++"),
        };
        private SubmissionInput Submission { get; set; } = new SubmissionInput();
        private string SubmitState = "Nộp bài";

        protected override async Task OnInitializedAsync()
        {
            Problem = await problemService.GetByIDAsync(ProblemID);
        }

        public async Task HandleSubmit(EditContext context)
        {
            SubmitState = "Đang xử lí";
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                if (context.Validate())
                {
                    SubmissionDTO result = await SubmissionService.SubmitCode(ProblemID, Submission);
                    if (result == null)
                    {
                        await ToastService.Error("Thông báo", "Lỗi bất định");
                    }
                    else
                    {
                        if (result.Status)
                        {
                            await ToastService.Success("Thông báo", "Bạn đã qua bài");
                        }
                        else
                        {
                            await ToastService.Error("Thông báo", "Lỗi");
                        }
                    }
                }
            }
            else
            {
                await ToastService.Error("Thông báo", "Chưa đăng nhập");
            }
            SubmitState = "Nộp bài";
        }

        public StandaloneEditorConstructionOptions GetOptions(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions()
            {
                Language = "cpp",
                AutomaticLayout = true,
                Scrollbar = new EditorScrollbarOptions()
                {
                    UseShadows = false,
                    Vertical = "Hidden",
                    AlwaysConsumeMouseWheel = false,
                    VerticalScrollbarSize = 0,
                    VerticalSliderSize = 0,
                    Horizontal = "Auto",
                },
                Theme = "vs-dark",
                ScrollBeyondLastColumn = 0,
                ScrollBeyondLastLine = false,
            };
        }
    }
}
