namespace Middleware.Web.Data;

public interface IEBridgeRepository
{
    Task<IReadOnlyList<ProductQtyRow>> GetProductQuantitiesAsync(CancellationToken ct);
    Task<IReadOnlyList<ProductRow>> GetProductsAsync(CancellationToken ct,string sku="");
    Task<IReadOnlyList<ImageSourceTypeRow>> GetProductImageSourceTypesAsync(CancellationToken ct);
    Task<IReadOnlyList<string>> GetProductNotAvailableOnWebAsync(CancellationToken ct);
    Task<IReadOnlyList<string>> GetDeletedProductSkusAsync(CancellationToken ct);
    Task<int> PaycorEmployeeSyncAsync(string json, CancellationToken ct);
    Task<IReadOnlyList<string>> GetLocationKeysByProductKeyAsync(string productKey, CancellationToken ct);    
    Task<string> GetPowerReviewInvoiceAsync();
    Task<string> GetChatMeterReviewFeedAsync();
    Task<ProductDeliveryDate> GetProductDeliveryDateAsync(string sku, string zipcode);
}
