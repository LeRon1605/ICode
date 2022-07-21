using BootstrapBlazor.Components;
using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Models;
using Web.Services;

namespace Web.Pages
{
    public partial class UserDetail
    {
        [Inject] private IUserService UserService { get; set; }
        [Inject] public ToastService ToastService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Parameter] public string ID { get; set; }
        private UserDTO User { get; set; }
        private UserUpdate Input { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Response<UserDTO> res = await UserService.FindByID(ID);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                User = res.Data;
                Input = new UserUpdate
                {
                    Username = User.Username
                };
            }
            else if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ToastService.Error("Thông báo", "Bạn chưa đăng nhập");
                NavigationManager.NavigateTo("/");
            }   
        }
        public async Task<Task> HandleSubmit(EditContext context)
        {
            Response<UserDTO> result = await UserService.UpdateUser(ID, Input);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                User = result.Data;
                Input.Username = User.Username;
                await ToastService.Success("Thông báo", "Cập nhật thành công");
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ToastService.Error("Thông báo", "Bạn chưa đăng nhập");
            }else if (result.StatusCode == HttpStatusCode.Forbidden)
            {
                await ToastService.Error("Thông báo", "Bạn không có quyền thực hiện tác vụ này");
            }
            else if (result.StatusCode == HttpStatusCode.Conflict)
            {
                await ToastService.Error("Thông báo", "Username đã tồn tại");
            }
            return Task.CompletedTask;
        }
    }
}
