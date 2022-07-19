using BootstrapBlazor.Components;
using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Web.Services;

namespace Web.Pages
{
    public partial class Login
    {
        [Inject] public IUserService UserService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public ToastService ToastService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private LoginUser User { get; set; } = new LoginUser();
        private bool? isError { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/");
            }    
        }
        public async Task LoginHandler(EditContext context)
        {
            if (context.Validate())
            {
                LoginResponse response = await UserService.Login(User);
                if (response.status)
                {
                    NavigationManager.NavigateTo("/");
                    await ToastService.Success("Thông báo", "Đăng nhập thành công", autoHide: true);
                }
                else
                {
                    isError = false;
                }    
            }    
        }
    }
}
