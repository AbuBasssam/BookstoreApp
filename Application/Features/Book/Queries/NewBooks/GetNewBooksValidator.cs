using Application.Validations;

namespace Application.Features.Book;

public class GetNewBooksValidator : LocalizePaginationValidator<GetNewBooksQuery>
{
    public GetNewBooksValidator() : base()
    {

    }
}