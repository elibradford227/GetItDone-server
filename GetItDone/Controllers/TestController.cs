using Microsoft.AspNetCore.Mvc;

namespace GetItDone.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("throw")]
    public IActionResult ThrowException()
    {
        throw new InvalidOperationException("This is a test exception");
    }
}