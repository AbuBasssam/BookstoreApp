using Application.Features.Book.Dtos;
using Application.Features.Book.Enums;
using Application.Features.Home;
using Application.Interfaces;
using Application.Models;
using Domain.AppMetaData;
using Domain.Entities;
using Domain.Enums;
using Domain.HelperClasses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Features.Book.Queries;

public class GetBookDetailsHandler : IRequestHandler<GetBookDetailsQuery, Response<BookDetailsDto>>
{
    private readonly IAuthService _authService;
    private readonly ICacheService _cacheService;
    private readonly IGenericRepository<BookView, int> _bookViewRepository;
    private readonly IGenericRepository<ReservationRecordView, int> _reservationRecordsRepository;
    private readonly IGenericRepository<BorrowingRecordView, int> _borrowingRecordsViewRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IGenericRepository<BookRating, int> _bookRatingRepository;
    private readonly ResponseHandler _responseHandler;
    private readonly HomePageSettings _settings;
    private readonly ISystemSettingsRepository _systemSettings;


    public GetBookDetailsHandler(IGenericRepository<BookView, int> bookViewRepository, ResponseHandler responseHandler,
        IAuthService authService, IGenericRepository<ReservationRecordView, int> reservationRecordsRepository,
        IGenericRepository<BorrowingRecordView, int> borrowingRecordsViewRepository, IBookRepository bookRepository,
        HomePageSettings settings, ICacheService cacheServic,
        IGenericRepository<BookRating, int> bookRatingRepository, ISystemSettingsRepository systemSettings)
    {
        _bookViewRepository = bookViewRepository;
        _responseHandler = responseHandler;
        _authService = authService;
        _reservationRecordsRepository = reservationRecordsRepository;
        _borrowingRecordsViewRepository = borrowingRecordsViewRepository;
        _bookRepository = bookRepository;
        _settings = settings;
        _cacheService = cacheServic;
        _bookRatingRepository = bookRatingRepository;
        _systemSettings = systemSettings;
    }

    public async Task<Response<BookDetailsDto>> Handle(GetBookDetailsQuery request, CancellationToken cancellationToken)
    {
        (JwtSecurityToken? obj, Exception? e) = _authService.GetJwtAccessTokenObjFromAccessTokenString(request.Token);
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        (int userId, Exception? ex) = _authService.GetUserIdFromJwtAccessTokenObj(obj);

        if (ex != null)
        {


        }

        var bookView = request.LangCode == "ar" ? await _GetArData(request.Id) : await _GetEnData(request.Id);
        if (bookView == null)
        {
            return _responseHandler.NotFound<BookDetailsDto>();
        }

        var cachedData = await _cacheService.GetAsync<HomePageRedisDto>(CacheKeys.HomePageData);
        DateOnly NewBookDateThreshold = DateOnly.Parse(cachedData!.LastUpdated.Date.ToShortDateString());
        NewBookSetting setting = new NewBookSetting
        {
            NewBooksDateThreshold = NewBookDateThreshold,
            NewBooksDaysThreshold = _settings.NewBooksDaysThreshold

        };
        var responseSetting = new ResponseSetting { Lang = request.LangCode, NewBookSetting = setting };

        var BorrowedBookDetailsResponse = await _BorrowedBookResponse(bookView, responseSetting, userId);
        if (BorrowedBookDetailsResponse != null) return BorrowedBookDetailsResponse;


        var OverdueBookDetailsResponse = await _OverdueBookResponse(bookView, responseSetting, userId);
        if (OverdueBookDetailsResponse != null) return OverdueBookDetailsResponse;


        var ReservedBookDetailsResponse = await _ReservedBookResponse(bookView, responseSetting, userId);
        if (ReservedBookDetailsResponse != null) return ReservedBookDetailsResponse;


        if (bookView.IsBorrowable) return await _BorrowableBookResponse(bookView, responseSetting, userId);

        if (bookView.IsReservable) return await _ReservableBookResponse(bookView, responseSetting, userId);

        return await _UnavailableBookResponse(bookView, responseSetting, userId);



    }

    private async Task<BookView?> _GetArData(int BookId)
    {
        return await _bookViewRepository.GetTableNoTracking()
            .Where(bv => bv.IsActive && bv.Id.Equals(BookId)).Select(v => new BookView
            {
                Id = v.Id,
                ISBN = v.ISBN,
                TitleAR = v.TitleAR,
                DescriptionAR = v.DescriptionAR,
                PublisherNameAR = v.PublisherNameAR,
                AuthorID = v.AuthorID,
                AuthorNameAR = v.AuthorNameAR,
                LanguageAR = v.LanguageAR,
                CategoryID = v.CategoryID,
                CategoryNameAR = v.CategoryNameAR,
                PageCount = v.PageCount,
                PublishDate = v.PublishDate,
                AvailabilityDate = v.AvailabilityDate,
                Position = v.Position,
                CoverImage = v.CoverImage,
                IsBorrowable = v.IsBorrowable,
                IsReservable = v.IsReservable,
                Rating = v.Rating,

            })
            .FirstOrDefaultAsync();

    }

    private async Task<BookView?> _GetEnData(int BookId)
    {
        return await _bookViewRepository.GetTableNoTracking()
            .Where(bv => bv.IsActive && bv.Id.Equals(BookId)).Select(v => new BookView
            {
                Id = v.Id,
                ISBN = v.ISBN,
                TitleEN = v.TitleEN,
                DescriptionEN = v.DescriptionEN,
                PublisherNameEN = v.PublisherNameEN,
                AuthorID = v.AuthorID,
                AuthorNameEN = v.AuthorNameEN,
                LanguageEN = v.LanguageEN,
                CategoryID = v.CategoryID,
                CategoryNameEN = v.CategoryNameEN,
                PageCount = v.PageCount,
                PublishDate = v.PublishDate,
                AvailabilityDate = v.AvailabilityDate,
                Position = v.Position,
                CoverImage = v.CoverImage,
                IsBorrowable = v.IsBorrowable,
                IsReservable = v.IsReservable,
                Rating = v.Rating,

            })
            .FirstOrDefaultAsync();
    }

    private async Task<Response<BookDetailsDto>?> _BorrowedBookResponse(BookView bookView, ResponseSetting setting, int userId)
    {
        var activeBorrowingRecord = await _borrowingRecordsViewRepository.GetTableNoTracking()
            .Where(br => br.BookId == bookView.Id && br.BorrowingStatus == enBorrowingStatus.Active && br.MemberID == userId)
            .FirstOrDefaultAsync();

        if (activeBorrowingRecord == null) return null;

        BookDetailsDto dto = await _BuildBookDetailsAsync(bookView, setting, enBookState.Borrowed, userId);
        var meta = new BorrowedData { DueDate = activeBorrowingRecord.DueDate };

        return _responseHandler.Success(dto, meta);


    }

    private async Task<Response<BookDetailsDto>> _BorrowableBookResponse(BookView bookView, ResponseSetting setting, int userId)
    {


        BookDetailsDto dto = await _BuildBookDetailsAsync(bookView, setting, enBookState.Borrowable, userId);
        var averageBookiBorrowingDayDuration = await _borrowingRecordsViewRepository
            .GetTableNoTracking()
            .Where(br => br.BookId == bookView.Id && br.BorrowingStatus == enBorrowingStatus.Returned)
            .AverageAsync(br => EF.Functions.DateDiffDay(br.BorrowingDate, br.ReturnDate));

        int estimatedDays = (int)Math.Ceiling(averageBookiBorrowingDayDuration ?? 15);

        var sysSetting = await _systemSettings.GetSettingsAsync();
        var meta = new BorrowableData
        {
            FinePerDay = sysSetting.FinePerDay,
            MaxBorrowingDuration = sysSetting.MaxLoanDays,
            PickupRequiredHours = (sysSetting.ReservationExpiryDays * 24),
            RecommededBorrowingDuration = estimatedDays

        };
        return _responseHandler.Success(dto, meta);


    }

    private async Task<Response<BookDetailsDto>> _ReservableBookResponse(BookView bookView, ResponseSetting setting, int userId)
    {
        BookDetailsDto dto = await _BuildBookDetailsAsync(bookView, setting, enBookState.Reservable, userId);

        var (estimatedAvailableDate, peopleAhead) = await _SimulateReservationEstimationAsync(bookView.Id, null);

        var meta = new ReservableData { EstimatedAvailableDate = estimatedAvailableDate, PeopleAhead = peopleAhead };

        return _responseHandler.Success(dto, meta);
    }

    private async Task<Response<BookDetailsDto>?> _ReservedBookResponse(BookView bookView, ResponseSetting setting, int userId)
    {

        var userReservation = await _reservationRecordsRepository.GetTableNoTracking()
       .Where(r =>
       r.BookID == bookView.Id
       && r.MemberID == userId
       && r.ReservationState != enReservationState.Inactive
       && r.ReservationState != enReservationState.PickupExpired).FirstOrDefaultAsync();

        if (userReservation == null) return null;

        BookDetailsDto dto = await _BuildBookDetailsAsync(bookView, setting, enBookState.Reserved, userId);


        if (userReservation.ReservationState == enReservationState.ReadyForPickup)
        {
            TimeSpan pickupRemainder = TimeSpan.FromSeconds(userReservation.RemainingPickupHours!.Value);
            /*  userReservation.ExpirationDate.HasValue
            ? userReservation.ExpirationDate.Value - DateTime.UtcNow
            : TimeSpan.Zero;*/

            var extraData = new ReservedData
            {
                PickupTimeRemainder = $"{(int)pickupRemainder.TotalHours:D2}:{pickupRemainder.Minutes:D2}:{pickupRemainder.Seconds:D2}"
            };

            return _responseHandler.Success(dto, extraData);
        }
        else
        {
            (DateTime estimatedDate, int peopleAhead) = await _SimulateReservationEstimationAsync(bookView.Id, userReservation.WaitingQueuePosition - 1);
            var extraData = new ReservableData
            {
                EstimatedAvailableDate = estimatedDate,
                PeopleAhead = peopleAhead
            };
            return _responseHandler.Success(dto, extraData);
        }



    }

    private async Task<Response<BookDetailsDto>?> _OverdueBookResponse(BookView bookView, ResponseSetting setting, int userId)
    {


        // Fetch user's active borrowings for this book that are overdue
        var overdueBorrowing = await _borrowingRecordsViewRepository
            .GetTableNoTracking()
            .Where(br => br.BookId == bookView.Id
                         && br.MemberID == userId
                         && br.BorrowingStatus == enBorrowingStatus.Overdue
            ).FirstOrDefaultAsync();

        if (overdueBorrowing == null) return null;

        BookDetailsDto dto = await _BuildBookDetailsAsync(bookView, setting, enBookState.Overdue, userId);


        // Calculate overdue days and fine
        int overdueDays = (int)Math.Ceiling((DateTime.UtcNow - overdueBorrowing.DueDate).TotalDays);
        var systemSettings = await _systemSettings.GetSettingsAsync();
        decimal fine = overdueDays * systemSettings.FinePerDay;

        // Build metadata
        var meta = new OverdueData
        {
            DueDate = overdueBorrowing.DueDate,
            EstimatedFine = fine
        };

        // Return response
        return _responseHandler.Success(dto, meta);

    }

    private async Task<Response<BookDetailsDto>> _UnavailableBookResponse(BookView bookView, ResponseSetting setting, int userId)
    {
        BookDetailsDto dto = await _BuildBookDetailsAsync(bookView, setting, enBookState.Unavailable, userId);

        return _responseHandler.Success(dto);

    }

    private BookDetailsDto _BuildDetailsDto(BookView bookView, ResponseSetting setting, enBookState bookState, IReadOnlyList<SimilarBookDto> simialrBooks, IReadOnlyList<AuthorBookDto> authorBooks, bool IsNewBook, int totalReaders, byte userRating)
    {
        return new BookDetailsDto(
                    bookView.Id,
                    _LocalizationValue(setting.Lang, bookView.TitleAR, bookView.TitleEN),
                    _LocalizationValue(setting.Lang, bookView.AuthorNameAR, bookView.AuthorNameEN),
                    _LocalizationValue(setting.Lang, bookView.DescriptionAR, bookView.DescriptionEN),
                    bookView.ISBN, bookState, bookView.CoverImage, IsNewBook,
                    _LocalizationValue(setting.Lang, bookView.PublisherNameAR, bookView.PublisherNameEN),
                    bookView.PublishDate.Date.Year.ToString(), bookView.PageCount,
                    _LocalizationValue(setting.Lang, bookView.LanguageAR, bookView.LanguageEN),
                    _LocalizationValue(setting.Lang, bookView.CategoryNameAR, bookView.CategoryNameEN),
                    Convert.ToDouble(bookView.Rating), userRating, totalReaders, simialrBooks.ToList(), authorBooks.ToList()
        );
    }

    private async Task<BookDetailsDto> _BuildBookDetailsAsync(BookView bookView, ResponseSetting setting, enBookState state, int userId)
    {
        var similarBooks = await _bookRepository.GetTopBorrowedByCategoryAsync(
            (enCategory)bookView.CategoryID, 7, setting.Lang, setting.NewBookSetting);

        var authorBooks = await _bookRepository.GetTopBorrowedByAuthorAsync(
            bookView.AuthorID, 7, setting.Lang, setting.NewBookSetting);

        bool isNewBook = await _bookRepository.IsNewBook(bookView.Id, setting.NewBookSetting);

        int totalReaders = await _borrowingRecordsViewRepository.GetTableNoTracking()
            .CountAsync(br => br.BookId == bookView.Id && br.BorrowingStatus == enBorrowingStatus.Returned);

        var userRating = await _bookRatingRepository.GetTableNoTracking()
            .Where(r => r.UserID == userId && r.BookID == bookView.Id)
            .Select(r => r.Rating)
            .FirstOrDefaultAsync();

        return _BuildDetailsDto(
            bookView,
            setting,
            state,
            similarBooks,
            authorBooks,
            isNewBook,
            totalReaders,
            userRating
        );
    }

    private string _LocalizationValue(string langCode, string arValue, string enValue) => langCode == "ar" ? arValue : enValue;

    /// <summary>
    /// Simulates reservation queue for a book and returns the estimated available date.
    /// </summary>
    private async Task<(DateTime estimatedAvailableDate, int peopleAhead)> _SimulateReservationEstimationAsync(int bookId, int? Ahead)
    {
        // 1) Get Due Dates for all active borrowings
        var activeBorrowingDueDates = await _borrowingRecordsViewRepository.GetTableNoTracking()
            .Where(br => br.BookId == bookId && br.BorrowingStatus == enBorrowingStatus.Active)
            .OrderBy(br => br.DueDate)
            .Select(br => br.DueDate)
            .ToListAsync();

        // Edge case: no borrowings
        if (!activeBorrowingDueDates.Any())
            return (DateTime.UtcNow, 0);

        // 2) Count all reservations ahead in queue
        int peopleAhead = Ahead == null ? await _reservationRecordsRepository
           .GetTableNoTracking()
           .Where(r => r.BookID == bookId && r.ReservationType == enReservationType.Waiting &&
               (r.ReservationState == enReservationState.Active || r.ReservationState == enReservationState.ExpiringSoon || r.ReservationState == enReservationState.ReadyForPickup))
           .CountAsync() : Ahead.Value;

        // 3) Calculate average borrowing duration
        var estimatedBorrowingDays = await _borrowingRecordsViewRepository.GetTableNoTracking()
            .Where(br => br.BookId == bookId && br.BorrowingStatus == enBorrowingStatus.Returned)
            .AverageAsync(br => EF.Functions.DateDiffDay(br.BorrowingDate, br.ReturnDate));

        int avgBorrowingDays = (int)Math.Ceiling(estimatedBorrowingDays ?? 14);

        if (peopleAhead == 0)
        {
            // If no one is waiting, earliest due date is the estimated available date
            return (activeBorrowingDueDates.First(), 0);
        }

        // 4) PriorityQueue simulation
        var pq = new PriorityQueue<DateTime, DateTime>();
        foreach (var due in activeBorrowingDueDates)
            pq.Enqueue(due, due);

        DateTime estimatedAvailableDate = DateTime.UtcNow;

        for (int i = 0; i < peopleAhead; i++)
        {
            var earliest = pq.Dequeue();
            var newAvailableDate = earliest.AddDays(avgBorrowingDays);
            pq.Enqueue(newAvailableDate, newAvailableDate);
            estimatedAvailableDate = newAvailableDate;
        }

        return (estimatedAvailableDate, peopleAhead);
    }

    private class ResponseSetting
    {
        public string Lang { get; set; }
        public NewBookSetting NewBookSetting { get; set; }


    }
    private class BorrowableData
    {
        public int MaxBorrowingDuration { get; set; }
        public int RecommededBorrowingDuration { get; set; }
        public int PickupRequiredHours { get; set; }
        public decimal FinePerDay { get; set; }
    }
    private class BorrowedData
    {
        public DateTime DueDate { get; set; }

    }
    private class ReservableData
    {
        public DateTime EstimatedAvailableDate { get; set; }
        public int PeopleAhead { get; set; }

    }
    private class ReservedData
    {
        public string PickupTimeRemainder { get; set; }

    }
    private class OverdueData
    {
        public DateTime DueDate { get; set; }
        public decimal EstimatedFine { get; set; }
    }



}


