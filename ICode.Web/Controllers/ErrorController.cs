using Microsoft.AspNetCore.Mvc;

namespace ICode.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error/{statusCode}")]
        public IActionResult ErrorPageHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 500:
                    ViewData["icon"] = "https://assets7.lottiefiles.com/packages/lf20_inueopnr.json";
                    ViewData["statusCode"] = 500;
                    ViewData["error"] = "INTERNAL SERVER ERROR";
                    ViewData["description"] = "Sorry, something went wrong.";
                    break;
                case 401:
                    ViewData["icon"] = "https://assets10.lottiefiles.com/temp/lf20_QYm9j9.json";
                    ViewData["statusCode"] = 401;
                    ViewData["error"] = "UNAUTHORIZED";
                    ViewData["description"] = "You are not allow to acccess to this page. Please make sure you are logged in.";
                    break;
                case 404:
                    ViewData["icon"] = "https://assets1.lottiefiles.com/packages/lf20_aN06YJCbME.json";
                    ViewData["statusCode"] = 404;
                    ViewData["error"] = "PAGE NOT FOUND";
                    ViewData["description"] = "Uh oh, this page could not be found.";
                    break;
            }
            return View();
        }
    }
}
