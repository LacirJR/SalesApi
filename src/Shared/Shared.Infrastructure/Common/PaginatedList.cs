using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Common;

public class PaginatedList<T>
{
    public IEnumerable<T> Data { get; }
    public int CurrentPage { get; }
    public int TotalItems { get; }
    public int TotalCount { get; }

    public PaginatedList(IEnumerable<T> items, int count, int currentPage, int pageSize)
    {
        CurrentPage = currentPage;
        TotalItems = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Data = items;
    }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalItems;
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
