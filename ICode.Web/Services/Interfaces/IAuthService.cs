using CodeStudy.Models;
using ICode.Web.Models.DTO;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthCredential> Login(LoginUser user);
        Task<bool> Register(RegisterUser user);
        Task<bool> RequestChangePassword(ForgetPassword data);
        Task<ServiceResponse<bool>> ChangePassword(string userId, string token, ForgetPasswordSubmit data);
    }
}
