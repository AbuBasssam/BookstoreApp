using Application.Features.Book;
using Application.Features.Book.Dtos;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;

namespace Implementations;

public class BookRepository : GenericRepository<Book, int>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<(ICollection<BookDto>, PagingMetadata)> GetHomePageBookByCategoryPageAsync(DateOnly NewBookDateThreshold, enCategory categoryId, int pageNumber = 1, int pageSize = 10, string LangCode = "en")
    {
        // Get connection from UnitOfWork
        var connection = _context.Database.GetDbConnection();
        var result = new List<BookDto>();
        PagingMetadata metadata = new PagingMetadata();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_GetPagedBooksByCategory";

            // Add parameters
            command.Parameters.Add(new SqlParameter("@CategoryID", categoryId));
            command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
            command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new SqlParameter("@LangCode", LangCode));
            command.Parameters.Add(new SqlParameter("@NewBookDateThreshold", NewBookDateThreshold));


            // Open connectiparameterName: on
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            // Execute reader
            using var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                result.Add(
                    new BookDto
                    {
                        BookId = Convert.ToInt32(reader["BookId"]),
                        Title = reader["Title"].ToString(),
                        Author = reader["Author"].ToString(),
                        CoverImageUrl = reader["CoverImageUrl"]?.ToString(),
                        IsNewBook = Convert.ToBoolean(reader["IsNewBook"]),
                    }
                );
            }
            if (await reader.NextResultAsync())
            {

                while (await reader.ReadAsync())
                {
                    metadata.TotalCount = Convert.ToInt32(reader["TotalCount"]);
                    metadata.TotalPages = Convert.ToInt32(reader["TotalPages"]);




                }

            }


        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing stored procedure SP_GetPagedBooksByCategory");

        }
        return (result, metadata);
    }

    public async Task<(ICollection<BookDto>, PagingMetadata)> GetHomePageNewestBooksPageAsync(LocalizePaginationInfo paginationInfo, NewBookSetting newBookSetting)
    {
        // Get connection from UnitOfWork
        var connection = _context.Database.GetDbConnection();
        var result = new List<BookDto>();
        PagingMetadata metadata = new PagingMetadata();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_GetPagedNewestBooks";

            // Add parameters
            command.Parameters.Add(new SqlParameter("@PageNumber", paginationInfo.PageNumber));
            command.Parameters.Add(new SqlParameter("@PageSize", paginationInfo.PageSize));
            command.Parameters.Add(new SqlParameter("@LangCode", paginationInfo.Lang));

            command.Parameters.Add(new SqlParameter("@NewBooksDateThreshold", newBookSetting.NewBooksDateThreshold));
            command.Parameters.Add(new SqlParameter("@NewBooksDaysThreshold", newBookSetting.NewBooksDaysThreshold));


            // Open connectiparameterName: on
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            // Execute reader
            using var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                result.Add(
                    new BookDto
                    {
                        BookId = Convert.ToInt32(reader["BookId"]),
                        Title = reader["Title"].ToString(),
                        Author = reader["Author"].ToString(),
                        CoverImageUrl = reader["CoverImageUrl"]?.ToString(),
                        IsNewBook = Convert.ToBoolean(reader["IsNewBook"]),
                    }
                );
            }
            if (await reader.NextResultAsync())
            {

                while (await reader.ReadAsync())
                {
                    metadata.TotalCount = Convert.ToInt32(reader["TotalCount"]);
                    metadata.TotalPages = Convert.ToInt32(reader["TotalPages"]);




                }

            }


        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing stored procedure SP_GetPagedBooksByCategory");

        }
        return (result, metadata);
    }

    public async Task<IReadOnlyList<AuthorBookDto>> GetTopBorrowedByAuthorAsync(int authorId, int take, string lang, NewBookSetting newestSetting)
    {


        var date = newestSetting.NewBooksDateThreshold.ToString("yyyy-MM-dd");

        var sqlQuery = $@"
        Declare @Lang CHAR(2)='{lang}';
        WITH BorrowCounts AS (
            SELECT 
                br.BookId,
                COUNT(*) AS TotalBorrows
            FROM vw_BorrowingRecord br
            GROUP BY br.BookId
        ),
        BooksByAuthor AS (
            SELECT
                b.BookID As BookId,
                TitleEN,
                TitleAR,
        		b.PublishDate as PublishDate,
                Rating,
                CoverImage ,
                TotalBorrows
            FROM vw_Books b 
            LEFT JOIN BorrowCounts bc ON b.BookID = bc.BookId 
            WHERE AuthorID = {authorId} AND IsActive = 1
        )
        SELECT TOP {take} BookId,
            CASE WHEN @Lang = 'ar' THEN TitleAR ELSE TitleEN END AS Title,
        	Year(PublishDate)as PublicationYear,
            Rating As AverageRating,
            CoverImage as CoverImageUrl,
        dbo.fn_IsNewBook('{date}', {newestSetting.NewBooksDaysThreshold},BookId) AS  IsNewBook
        FROM BooksByAuthor 
        ORDER BY TotalBorrows DESC";
        var result = await _context.Database
           .SqlQueryRaw<AuthorBookDto>(sqlQuery)
           .AsNoTracking()
           .ToListAsync();

        return result;

    }

    public async Task<IReadOnlyList<SimilarBookDto>> GetTopBorrowedByCategoryAsync(enCategory categoryId, int take, string lang, NewBookSetting newestSetting)
    {
        var date = newestSetting.NewBooksDateThreshold.ToString("yyyy-MM-dd");

        var sqlQuery = $@"
Declare @Lang CHAR(2)='{lang}';
WITH BorrowCounts AS (
    SELECT 
        br.BookId,
        COUNT(*) AS TotalBorrows
    FROM vw_BorrowingRecord br
    GROUP BY br.BookId
),
BooksByCategory AS (
    SELECT
        b.BookID As BookId,
        TitleEN,
        TitleAR,
        AuthorNameEN,
        AuthorNameAR,
        Rating,
        CoverImage ,
        TotalBorrows
    FROM vw_Books b 
    LEFT JOIN BorrowCounts bc ON b.BookID = bc.BookId 
    WHERE CategoryID = {(int)categoryId} AND IsActive = 1
)
SELECT TOP {take}
    BookId,
    CASE WHEN @Lang = 'ar' THEN TitleAR ELSE TitleEN END AS Title,
    CASE WHEN @Lang = 'ar' THEN AuthorNameAR ELSE AuthorNameEN END AS Author,
    Rating As AverageRating,
    CoverImage as CoverImageUrl,
dbo.fn_IsNewBook('{date}', {newestSetting.NewBooksDaysThreshold},BookId) AS  IsNewBook
FROM BooksByCategory
ORDER BY TotalBorrows DESC";

        var result = await _context.Database
            .SqlQueryRaw<SimilarBookDto>(sqlQuery)
            .AsNoTracking()
            .ToListAsync();

        return result;

    }

    public async Task<bool> IsNewBook(int bookId, NewBookSetting newestSetting)
    {
        var connection = _context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT dbo.fn_IsNewBook(@date, @days, @bookId)";
        cmd.Parameters.Add(new SqlParameter("@date", newestSetting.NewBooksDateThreshold));
        cmd.Parameters.Add(new SqlParameter("@days", newestSetting.NewBooksDaysThreshold));
        cmd.Parameters.Add(new SqlParameter("@bookId", bookId));

        var scalarResult = await cmd.ExecuteScalarAsync();
        return (bool)scalarResult;
    }
}
