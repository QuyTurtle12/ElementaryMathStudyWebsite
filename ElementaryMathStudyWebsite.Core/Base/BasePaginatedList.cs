using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Base
{
    public class BasePaginatedList<T>
    {

        public IReadOnlyCollection<T> Items { get; private set; }

        // Property to store the total number of items
        public int TotalItems { get; private set; }

        // Property to store the current page number
        public int CurrentPage { get; private set; }

        // Property to store the total number of pages
        public int TotalPages { get; private set; }

        // Property to store the number of items per page
        public int PageSize { get; private set; }

        // Constructor to initialize the paginated list
        public BasePaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
        {
            TotalItems = count;
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (pageSize > 0) ? (int)Math.Ceiling(count / (double)pageSize) : (count > 0 ? 1 : 0);
            Items = items;
        }

        // Method to check if there is a previous page
        public bool HasPreviousPage => CurrentPage > 1;

        // Method to check if there is a next page
        public bool HasNextPage => CurrentPage < TotalPages;

        public static implicit operator BasePaginatedList<T>(BasePaginatedList<Topic?> v)
        {
            throw new NotImplementedException();
        }

        public static BasePaginatedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count(); // Tổng số phần tử
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); // Lấy dữ liệu theo trang
            return new BasePaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
