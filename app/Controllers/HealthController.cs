using Microsoft.AspNetCore.Mvc;

namespace TawakalApi.app.Controllers;

[ApiController]
[Route("test")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("TestController is working!");
    }   
}