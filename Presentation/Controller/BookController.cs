using Application.Features.Book;
using Application.Features.Book.Dtos;
using Application.Features.Book.Queries;
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
    public async Task<IActionResult> GetBooksByCategory(enCategory id, int pageNumber = 1, int pageSize = 10)
    {
        var langCode = HttpContext.GetRequestLanguage();
        var queyr = new GetBooksByCategoryQuery(id, pageNumber, pageSize, langCode);
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

    [HttpGet(Router.BookRouter.Get)]
    public async Task<IActionResult> GetBookDetails(int id)
    {
        var langCode = HttpContext.GetRequestLanguage();
        var token = HttpContext.GetAuthToken();
        var queyr = new GetBookDetailsQuery(id, langCode, token!);

        return await QueryExecutor.Execute(
            queyr,
            Sender,
            (Response<BookDetailsDto> response) => NewResult(response)
        );

    }


}