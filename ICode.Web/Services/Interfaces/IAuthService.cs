using CodeStudy.Models;
using ICode.Web.Models.DTO;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthCredential> Login(LoginUser user);
        Task<AuthCredential> LoginByGoogle(string access_token);
        Task<AuthCredential> RefreshToken(string access_token, string refresh_token);
        Task<GoogleTokenResponse> GetGoogleToken(string code);
        Task<UserDTO> GetProfile(string access_token);
        Task<bool> Register(RegisterUser user);
        Task<bool> RequestChangePassword(ForgetPassword data);
        Task<ServiceResponse<bool>> ChangePassword(string userId, string token, ForgetPasswordSubmit data);
    }
}
