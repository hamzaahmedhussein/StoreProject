using API.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("errors/{code}")]
     [ApiExplorerSettings(IgnoreApi =true)]
    public class ErrorController : BaseApiController
    {
        [HttpGet]

        public IActionResult Error(int code)
        {
            return new ObjectResult(new ApiRespose(code));
        }
    }
}
