namespace Middleware.Web.Models.Common;

/// <summary>
/// Represents a paging request model
/// </summary>
public  interface IPagingRequestModel
{
    /// <summary>
    /// Gets a page number
    /// </summary>
    int Page { get; }

    /// <summary>
    /// Gets a page size
    /// </summary>
    int PageSize { get; }
}
