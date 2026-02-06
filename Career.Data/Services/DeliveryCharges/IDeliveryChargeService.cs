using Career.Data.Domains.DeliveryCharges;
using System.Threading.Tasks;

namespace Career.Data.Services.DeliveryCharges;

public interface IDeliveryChargeService
{
    /// <summary>
    /// Gets the delivery charge by zip
    /// </summary>
    /// <param name="zip">The zip</param>
    Task<DeliveryCharge> GetDeliveryChargeByZipPostalCodeAsync(string zipPostalCode);
}
