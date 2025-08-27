namespace Application.Models;

/// <summary>
/// Generic paged result wrapper for API responses
/// </summary>
/// <typeparam name="T">Type of items in the collection</typeparam>
public class PagedResult<T>
{

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; internal set; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; internal set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; internal set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; internal set; }

    /// <summary>
    /// Additional metadata for the paged result
    /// </summary>
    public object? Metadata { get; internal set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// Whether this is the first page
    /// </summary>
    public bool IsFirst => Page == 1;

    /// <summary>
    /// Whether this is the last page
    /// </summary>
    public bool IsLast => Page == TotalPages;

    /// <summary>
    /// Number of items in the current page
    /// </summary>
    public int CurrentPageSize => Items?.Count ?? 0;

    /// <summary>
    /// Whether the result has any items
    /// </summary>
    public bool HasItems => Items?.Any() == true;

    /// <summary>
    /// Whether the result is empty
    /// </summary>
    public bool IsEmpty => !HasItems;

    /// <summary>
    /// The items in the current page
    /// </summary>
    public List<T> Items { get; internal set; } = new();


    // Private constructor - force use of Builder
    internal PagedResult() { }

    /// <summary>
    /// Create a new PagedResult builder
    /// </summary>
    /// <returns>PagedResult builder instance</returns>
    public static PagedResultBuilder<T> Builder() => new PagedResultBuilder<T>();

    /// <summary>
    /// Create an empty paged result quickly
    /// </summary>
    public static PagedResult<T> Empty(int page = 1, int pageSize = 10) =>
        Builder()
            .WithPage(page)
            .WithPageSize(pageSize)
            .WithItems(new List<T>())
            .WithTotalCount(0)
            .Build();

    /// <summary>
    /// Create a paged result from a collection quickly (backward compatibility)
    /// </summary>
    public static PagedResult<T> Create(List<T> items, int totalCount, int page, int pageSize) =>
        Builder()
            .WithItems(items)
            .WithTotalCount(totalCount)
            .WithPage(page)
            .WithPageSize(pageSize)
            .Build();
}
