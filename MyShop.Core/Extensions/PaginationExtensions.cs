using MyShop.Core.DTOs;

namespace MyShop.Core.Extensions
{
    public static class PaginationExtensions
    {
        public static PaginatedResult<T> ToPaginated<T>(
            this IEnumerable<T> source,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 1)
                pageSize = 10;
            if (pageSize > 100)
                pageSize = 100;

            var totalItems = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<T>(items, pageNumber, pageSize, totalItems);
        }
    }
}
