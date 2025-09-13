using Application.Validations;

namespace Application.Features.Book;

public class GetBooksByCategoryValidator : LocalizePaginationValidator<GetNewBooksQuery>
{
    public GetBooksByCategoryValidator() : base()
    {

    }
}