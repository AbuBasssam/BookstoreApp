using Domain.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controller;

public class TestController : ApiController
{
    [HttpGet(Router.TestRouter.Global)]
    public IActionResult GlobalGetRateLimitedResponse()
    {
        return Ok("This is a test response from a Global rate-limited endpoint.");
    }
    [HttpGet(Router.TestRouter.Sensitive)]
    public IActionResult SensitiveGetRateLimitedResponse()
    {
        return Ok("This is a test response from a Sensitive rate-limited endpoint.");
    }

}