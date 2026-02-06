namespace Middleware.Web.Data;

public interface INopRepository
{
    Task<int> SyncProductQuantitiesAsync(IReadOnlyList<(string Sku, int StockQty)> rows,
        CancellationToken ct);
    Task<int> UpsertProductsAsync(IReadOnlyList<ProductRow> rows, int batchSize, CancellationToken ct);

    Task<int> DeleteProductsBySkuAsync(IReadOnlyList<string> skus, int batchSize, CancellationToken ct);

    Task<int> UpdateImageSourceTypeAsync(IReadOnlyList<ImageSourceTypeRow> rows, int batchSize,
        CancellationToken ct);

    Task<int> UpdateNotAvailableOnWebAsync(IReadOnlyList<string> skus, int batchSize, CancellationToken ct);
}
