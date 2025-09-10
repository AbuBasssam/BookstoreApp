using Application.Features.Home;
using Application.Interfaces;
using Domain.AppMetaData;
using Domain.Entities;
using Domain.Enums;
using Domain.HelperClasses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;

namespace Infrastructure.Seeder;

public class HomePageDataSeeder
{
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _dbContext;
    private readonly HomePageSettings _homePage;

    public HomePageDataSeeder(ICacheService cacheService, AppDbContext dbContext, HomePageSettings homePage)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
        _homePage = homePage;
    }

    public async Task SeedAsync()
    {
        string CacheKey = CacheKeys.HomePageData;
        // 1. تحقق من وجود البيانات في الكاش
        var exists = await _cacheService.ExistsAsync(CacheKey);
        if (exists)
        {
            //Log.Information("HomePageData already cached, skipping seeding.");
            return;
        }

        // 2. get Data from  Stored Procedure
        //Log.Information("Fetching HomePageData from database...");
        var homePageData = await GetHomePageDataFromDbAsync();

        if (homePageData == null)
        {
            Log.Warning("SP_GetHomePageData returned null, skipping cache set.");
            return;
        }

        // 3. Save to cache with expiration
        await _cacheService.SetAsync(CacheKey, homePageData, TimeSpan.FromDays(_homePage.CacheExpirationDays));


        Log.Information("HomePageData cached successfully.");
    }

    private async Task<HomePageRedisDto> GetHomePageDataFromDbAsync()
    {
        // Get connection from UnitOfWork
        var connection = _dbContext.Database.GetDbConnection();
        var result = new HomePageRedisDto();
        try
        {
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_GetHomePageData";


            // Add parameters
            command.Parameters.Add(new SqlParameter("@NewBooksDaysThreshold", _homePage.NewBooksDaysThreshold));
            command.Parameters.Add(new SqlParameter("@PopularityDaysThreshold", _homePage.PopularityDaysThreshold));
            command.Parameters.Add(new SqlParameter("@PopularBooksCount", _homePage.MostPopularBooksCount));


            // Open connectiparameterName: on
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            // Execute reader
            using var reader = await command.ExecuteReaderAsync();

            var categories = new List<Category>();
            while (await reader.ReadAsync())
            {
                categories.Add(
                    new Category
                    {
                        Id = (enCategory)Convert.ToInt32(reader["CategoryId"]),
                        NameEN = reader["CategoryNameEN"].ToString(),
                        NameAR = reader["CategoryNameAR"].ToString()
                    }
                );
            }
            result.CategoriesData = categories;
            // Move to Books result set
            if (await reader.NextResultAsync())
            {
                var books = new List<BookRedisDto>();
                while (await reader.ReadAsync())
                {
                    books.Add(
                        new BookRedisDto
                        {
                            Id = Convert.ToInt32(reader["BookId"]),
                            TitleEN = reader["TitleEN"].ToString(),
                            TitleAR = reader["TitleAR"].ToString(),
                            CoverImage = reader["CoverImageUrl"].ToString(),
                            IsFirstCategory = Convert.ToBoolean(reader["IsFirstCategory"]),
                            IsNewBook = Convert.ToBoolean(reader["IsNewBook"]),
                            IsMostPopular = Convert.ToBoolean(reader["IsMostPopular"]),
                            Author = new AuthorRedisDto
                            {
                                NameEN = reader["AuthorNameEN"].ToString(),
                                NameAR = reader["AuthorNameAR"].ToString()
                            }
                        }
                    );
                }
                result.pageData = books;
            }

            // Move to page meta data result set
            if (await reader.NextResultAsync())
            {
                var metaData = new HomePageMetaDto();
                while (await reader.ReadAsync())
                {

                    metaData.FirstCategoryPageSize = Convert.ToInt32((reader["FirstCategoryPageSize"]));
                    metaData.TotalFirstCategoryBooks = Convert.ToInt32((reader["TotalFirstCategoryBooks"]));
                    metaData.NewBooksPageSize = Convert.ToInt32((reader["NewBooksPageSize"]));
                    metaData.TotalNewBooks = Convert.ToInt32((reader["TotalNewBooks"]));

                }
                result.MetaData = metaData;



            }
            result.LastUpdated = DateTime.UtcNow;



        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing stored procedure SP_GetHomePageData");

        }
        return result;
    }
    private class BookRedisDto
    {
        public int Id { get; set; }
        public string TitleEN { get; set; }
        public string TitleAR { get; set; }
        public string CoverImage { get; set; }
        public bool IsFirstCategory { get; set; }
        public bool IsNewBook { get; set; }
        public bool IsMostPopular { get; set; }
        public AuthorRedisDto Author { get; set; }
    }
    private class AuthorRedisDto
    {
        public string NameEN { get; set; }
        public string NameAR { get; set; }
    }
    private class HomePageRedisDto
    {
        public List<Category> CategoriesData { get; set; }
        public List<BookRedisDto> pageData { get; set; }
        public HomePageMetaDto MetaData { get; set; }
        public DateTime LastUpdated { get; set; }

    }

}
