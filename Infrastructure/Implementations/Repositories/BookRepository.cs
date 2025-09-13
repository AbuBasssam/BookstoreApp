using Application.Features.Book;
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


}
