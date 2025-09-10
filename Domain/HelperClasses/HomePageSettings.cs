namespace Domain.HelperClasses;

public class HomePageSettings
{
    public int NewBooksDaysThreshold { get; set; } // Number of days to consider a book as "new"
    public int MostPopularBooksCount { get; set; } // Number of most popular books to display  
    public int PopularityDaysThreshold { get; set; } // Number of days to consider for popularity calculation
    public int CacheExpirationDays { get; set; } // Number of days to cache the homepage data 
}