using Application.Features.Book;
using Application.Models;
using Domain.AppMetaData;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Helpers;

namespace Presentation.Controller;

public class BookController : ApiController
{

    [HttpGet(Router.BookRouter.ByCategory)]
    public async Task<IActionResult> GetBooksByCategory(enCategory categoryId, int pageNumber = 1, int pageSize = 10)
    {
        var langCode = HttpContext.GetRequestLanguage().ToLower();
        var queyr = new GetBooksByCategoryQuery(categoryId, langCode.Equals("ar") ? "ar" : "en", pageNumber, pageSize);
        return await QueryExecutor.Execute(
            queyr,
            Sender,
            (Response<PagedResult<BookDto>> response) => NewResult(response)
        );




    }

    [HttpGet(Router.BookRouter.Newest)]
    public async Task<IActionResult> GetNewestBooksPage(int pageNumber = 1, int pageSize = 10)
    {
        var langCode = HttpContext.GetRequestLanguage();
        var queyr = new GetNewBooksQuery(pageNumber, pageSize, langCode);

        return await QueryExecutor.Execute(
            queyr,
            Sender,
            (Response<PagedResult<BookDto>> response) => NewResult(response)
        );




    }


}