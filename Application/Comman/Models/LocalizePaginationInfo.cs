namespace Application.Models;

public class LocalizePaginationInfo
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string Lang { get; set; } = "en";
}
