using Middleware.Web.Domains;
using Middleware.Web.Domains.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Common;

/// <summary>
/// Generic attribute service interface
/// </summary>
public interface IGenericAttributeService
{   
    /// <summary>
    /// Get attributes
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="keyGroup">Key group</param>
    /// <returns>Get attributes</returns>
    Task<IList<GenericAttribute>> GetAttributesForEntityAsync(int entityId, string keyGroup);

    /// <summary>
    /// Get an attribute of an entity
    /// </summary>
    /// <typeparam name="TPropType">Property type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="key">Key</param>
    /// <param name="keyGroup">keyGroup</param>
    /// <param name="storeId">Load a value specific for a certain store; pass 0 to load a value shared for all stores</param>
    /// <returns>Attribute</returns>
    Task<TPropType> GetAttributeAsync<TPropType>(BaseEntity entity, string key, string keyGroup, int storeId = 0);
}
