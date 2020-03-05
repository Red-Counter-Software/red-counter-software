namespace RedCounterSoftware.Common
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchParameters{T}"/> class.
    /// </summary>
    /// <param name="searchTerm">The term to search for.</param>
    /// <param name="sortTerm">The name of the property to sort by.</param>
    /// <param name="isDescending">True for a descending order, false for ascending.</param>
    /// <param name="pageSize">The amount of items to return.</param>
    /// <param name="currentPage">The 0 based index of the subset of items to return.</param>
    public class SearchParameters
    {
        public SearchParameters()
        {
            this.SearchTerm = string.Empty;
            this.SortTerm = string.Empty;
        }

        public SearchParameters(string searchTerm, string sortTerm, bool isDescending = false, int pageSize = 10, int currentPage = 0)
        {
            this.SearchTerm = searchTerm ?? string.Empty;
            this.SortTerm = sortTerm ?? string.Empty;
            this.IsDescending = isDescending;
            this.PageSize = pageSize >= 10 ? pageSize : 10;
            this.CurrentPage = currentPage >= 0 ? currentPage : 0;
        }

        /// <summary>
        /// Gets or sets the term to search for.
        /// </summary>
        /// <value>
        /// The term to search for.
        /// </value>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the name of the property to sort by.
        /// </summary>
        /// <value>
        /// The term to sort by.
        /// </value>
        public string SortTerm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order is descending or ascending.
        /// </summary>
        /// <value>
        /// True for descending order, false for ascending.
        /// </value>
        public bool IsDescending { get; set; }

        /// <summary>
        /// Gets or sets the amount of items to return.
        /// </summary>
        /// <value>
        /// The amount of items to return.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the 0 based index of the subset of items to return.
        /// </summary>
        /// <value>
        /// The 0 based index of the subset of items to return.
        /// </value>
        public int CurrentPage { get; set; }
    }
}
