using Middleware.Web.Domains.DeliveryCharges;
using System.Threading.Tasks;

namespace Middleware.Web.Services.DeliveryCharges;

public interface IDeliveryChargeService
{
    /// <summary>
    /// Gets the delivery charge by zip
    /// </summary>
    /// <param name="zip">The zip</param>
    Task<DeliveryCharge> GetDeliveryChargeByZipPostalCodeAsync(string zipPostalCode);
}
