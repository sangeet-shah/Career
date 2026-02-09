using Middleware.Web.Data;
using Middleware.Web.Domains.DeliveryCharges;
using Dapper;
using Middleware.Web.Data;
using System.Threading.Tasks;

namespace Middleware.Web.Services.DeliveryCharges;

public class DeliveryChargeService : IDeliveryChargeService
{
    private const string DeliveryChargeTable = "FM_DeliveryCharge";

    private readonly DbConnectionFactory _db;

    public DeliveryChargeService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<DeliveryCharge> GetDeliveryChargeByZipPostalCodeAsync(string zipPostalCode)
    {
        if (string.IsNullOrEmpty(zipPostalCode))
            return null;

        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{DeliveryChargeTable}] WHERE ZipPostalCode = @ZipPostalCode";
        return await conn.QueryFirstOrDefaultAsync<DeliveryCharge>(sql, new { ZipPostalCode = zipPostalCode });
    }
}
