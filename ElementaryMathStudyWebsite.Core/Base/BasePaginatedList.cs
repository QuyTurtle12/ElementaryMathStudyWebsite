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
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
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
    }
}
