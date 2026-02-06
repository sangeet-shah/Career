namespace Middleware.Web.Data;

public sealed record ProductQtyRow(string Sku, int StockQty);

public sealed record ProductRow
{
    public string Id { get; init; } = "";
    public string? OriginalDescription { get; init; }
    public string? VendorModelNumber { get; init; }
    public string? VendorUPCCode { get; init; }
    public decimal CurrentPrice { get; init; }
    public decimal Msrp { get; init; }
    public decimal AverageCost { get; init; }
    public decimal Weight { get; init; }
    public decimal Depth { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public DateTime? BeginningPromoDate { get; init; }
    public DateTime? EndingPromoDate { get; init; }
    public decimal? PromoPrice { get; init; }
    public string? BrandDescirption { get; init; }
    public string? BrandId { get; init; }
    public string? VendorName { get; init; }
    public string? VendorId { get; init; }
    public string? Status { get; init; }
    public string? GroupID { get; init; }
    public DateTime? DateChanged { get; init; }
    public DateTime? DateCreated { get; init; }
}

public sealed record ProductSyncResult(
    int UpdatedCount,
    int InsertedCount,
    int SkippedBlockedCount,
    int InputCount);



public sealed record ImageSourceTypeRow(string ProductID, int ImageSourceType);

internal sealed class PaycorTokenCache
{
    public string AccessToken { get; init; } = "";
    public DateTimeOffset ExpiresAt { get; init; }
}

public sealed class PaycorAPITokenResponse
{
    [Newtonsoft.Json.JsonProperty("access_token")]
    public string AccessToken { get; set; } = "";

    [Newtonsoft.Json.JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}

public sealed record ProductDeliveryDate(string RouteCodeID, DateTime AvailableDeliveryDates,string WhenCanYouHaveIt);