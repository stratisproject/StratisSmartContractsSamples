using Microsoft.AspNetCore.Mvc;
using Signature.Shared.Models;

namespace Signature.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IActionResult JsonError(string errorMessage)
        {
            return Json(new AjaxResponse
            {
                Success = false,
                Message = errorMessage
            });
        }
        protected IActionResult JsonSuccess(string message = "", object data = null)
        {
            return Json(new AjaxResponse
            {
                Success = true,
                Message = message,
                Data = data
            });
        }
    }
}