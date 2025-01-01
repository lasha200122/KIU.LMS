namespace KIU.LMS.Domain.Common.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int itemsPerPage)
    {
        return query.Skip(itemsPerPage * (pageNumber - 1)).Take(itemsPerPage);
    }
}
