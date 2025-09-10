using Domain.AppMetaData;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controller;

public class BookController : ApiController
{

    [HttpGet(Router.BookRouter.GetByCategory)]
    public async Task<IActionResult> GetBooksByCategory(enCategory categoryId)
    {
        throw new NotImplementedException();


    }
}