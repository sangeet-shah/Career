using Career.Data.Data;
using Career.Data.Domains.DeliveryCharges;
using Career.Data.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.DeliveryCharges;

public class DeliveryChargeService : IDeliveryChargeService
{
    #region Fields

    private readonly IRepository<DeliveryCharge> _deliveryChargeRepository;

    #endregion

    #region Ctor

    public DeliveryChargeService(IRepository<DeliveryCharge> deliveryChargeRepository)
    {
        _deliveryChargeRepository = deliveryChargeRepository;
    }

    #endregion

    #region Methods 

    /// <summary>
    /// Gets the delivery charge by zip
    /// </summary>
    /// <param name="zip">The zip</param>
    public async Task<DeliveryCharge> GetDeliveryChargeByZipPostalCodeAsync(string zipPostalCode)
    {
        if (string.IsNullOrEmpty(zipPostalCode))
            return null;

        return await _deliveryChargeRepository.Table.Where(x => x.ZipPostalCode == zipPostalCode).FirstOrDefaultAsync();                    
    }

    #endregion
}
