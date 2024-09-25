namespace ElementaryMathStudyWebsite.Core.Utils
{
    public class PaginationHelper
    {
        public static int ValidateAndAdjustPageNumber(int pageNumber, int totalItems, int pageSize)
        {
            // Calculate total number of pages
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // If there are no items, reset the page number to 1
            if (totalItems == 0)
            {
                return 1; // You may choose to return 0 or another value if you prefer
            }

            // Adjust page number if it exceeds total pages
            if (pageNumber > totalPages)
            {
                return totalPages; // Set to last page if requested page exceeds total
            }

            // Return the original page number if valid
            return pageNumber <= 0 ? 1 : pageNumber; // Ensure page number is at least 1
        }
    }
}
