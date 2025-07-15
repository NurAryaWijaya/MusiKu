using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MusiKu_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("MusiKu Backend is working!");
        }
    }
}
