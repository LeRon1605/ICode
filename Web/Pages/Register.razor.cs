using CodeStudy.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Services;

namespace Web.Pages
{
    public partial class Register
    {
        [Inject]
        private IUserService userService { get; set; }
        private RegisterUser User = new RegisterUser();
        private bool? result;
        private async Task HandleSubmit(EditContext editContext)
        {
            if (editContext.Validate())
            {
                result = await userService.Register(User);
                if ((bool)result) User = new RegisterUser();
            }    
        }
    }
}
