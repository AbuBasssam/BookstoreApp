using Application.Features.Home;
using Application.Models;
using Domain.AppMetaData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Helpers;

namespace Presentation.Controller;

public class HomePageController : ApiController
{
    [HttpGet(Router.HomeRouter.BASE)]
    public async Task<IActionResult> GetHomePageData()
    {
        var langCode = HttpContext.GetRequestLanguage();
        var token = HttpContext.GetAuthToken();
        var queyr = new GetHomePageDataQuery(token!, langCode);
        return await QueryExecutor.Execute(
            queyr,
            Sender,
            (Response<HomePageResponseDto> response) => NewResult(response)
        );
    }
}