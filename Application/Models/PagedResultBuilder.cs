namespace Application.Models;

public class PagedResultBuilder<T> 
{
    private readonly PagedResult<T> _result = new();
    private bool _built = false;

    public PagedResultBuilder<T> WithItems(List<T> items)
    {
        ValidateNotBuilt();
        _result.Items = items ?? new List<T>();
        return this;
    }

    public PagedResultBuilder<T> WithItems(IEnumerable<T> items)
    {
        ValidateNotBuilt();
        _result.Items = items?.ToList() ?? new List<T>();
        return this;
    }

    public PagedResultBuilder<T> WithTotalCount(int totalCount)
    {
        ValidateNotBuilt();
        if (totalCount < 0)
            throw new ArgumentException("Total count cannot be negative", nameof(totalCount));

        _result.TotalCount = totalCount;
        return this;
    }

    public PagedResultBuilder<T> WithPage(int page)
    {
        ValidateNotBuilt();
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0", nameof(page));

        _result.Page = page;
        return this;
    }

    public PagedResultBuilder<T> WithPageSize(int pageSize)
    {
        ValidateNotBuilt();
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        _result.PageSize = pageSize;
        return this;
    }

    public PagedResultBuilder<T> WithMetadata(object metadata)
    {
        ValidateNotBuilt();
        _result.Metadata = metadata;
        return this;
    }

    public PagedResultBuilder<T> WithMetadata<TMeta>(TMeta metadata)
    {
        ValidateNotBuilt();
        _result.Metadata = metadata;
        return this;
    }

    public PagedResultBuilder<T> AddItem(T item)
    {
        ValidateNotBuilt();
        _result.Items ??= new List<T>();
        _result.Items.Add(item);
        return this;
    }

    public PagedResultBuilder<T> AddItems(IEnumerable<T> items)
    {
        ValidateNotBuilt();
        if (items != null)
        {
            _result.Items ??= new List<T>();
            _result.Items.AddRange(items);
        }
        return this;
    }

    public PagedResult<T> Build()
    {
        ValidateNotBuilt();
        ValidateRequiredFields();

        // Calculate total pages
        _result.TotalPages = _result.PageSize > 0
            ? (int)Math.Ceiling((double)_result.TotalCount / _result.PageSize)
            : 0;

        _built = true;
        return _result;
    }

    private void ValidateNotBuilt()
    {
        if (_built)
            throw new InvalidOperationException("Builder has already been used to build a result");
    }

    private void ValidateRequiredFields()
    {
        if (_result.Page == 0)
            throw new InvalidOperationException("Page must be set");

        if (_result.PageSize == 0)
            throw new InvalidOperationException("PageSize must be set");

        // TotalCount can be 0 for empty results, so we don't validate it
        // Items can be null/empty for empty results, so we don't validate them
    }
}