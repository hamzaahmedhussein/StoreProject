//using API.Errors;
//using Infrastructure.Data;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace API.Controllers
//{
//    public class BuggyController : BaseApiController
//    {
//        private readonly ApplicationDbContext _context;
//        public BuggyController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("testauth")]
//        [Authorize]
//        public ActionResult<string> GetSecretText()
//        {
//            return "secret stuff";
//        }

//        [HttpGet("notfound")]
//        public ActionResult GetNotFoundRequest()
//        {
//            var thing = _context.Products.Find(42);

//            if (thing == null)
//                return NotFound(new ApiResponse(404));

//            return Ok();
//        }

//        [HttpGet("servererror")]
//        public ActionResult GetServerError()
//        {
//            var thing = _context.Products.Find(42);

//            var thingToReturn = thing.ToString();

//            return Ok();
//        }

//        [HttpGet("badrequest")]

//        public ActionResult GetBadRequest()
//        {
//            return BadRequest();
//        }

//        [HttpGet("badrequest/{id}")]
//        public ActionResult GetNotFoundRequest(int id)
//        {
//            return Ok();
//        }
//    }
//}