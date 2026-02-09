namespace Middleware.Web.Models.Common;

/// <summary>
/// Represents base search model
/// </summary>
public record BaseSearchModel : IPagingRequestModel
{
    #region Properties

    /// <summary>
    /// Gets a page number
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets a page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets a comma-separated list of available page sizes
    /// </summary>
    public string? AvailablePageSizes { get; set; }

    /// <summary>
    /// Gets or sets draw. Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence).
    /// </summary>
    public string? Draw { get; set; }


    /// <summary>
    /// Gets or sets total count
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets Total page
    /// </summary>
    public int TotalPages { get; set; }

    #endregion       
}
