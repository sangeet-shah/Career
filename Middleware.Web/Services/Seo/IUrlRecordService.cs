using Career.Data.Domains.Seo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Seo;

public interface IUrlRecordService
{
    /// <summary>
    /// Find URL record
    /// </summary>
    /// <param name="slug">Slug</param>
    /// <param name="storeid">storeId</param>
    /// <returns>Found URL record</returns>
    Task<UrlRecord> GetBySlugAsync(string slug, int storeId = 0);

    /// <summary>
    /// Get search engine friendly name (slug)
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>        
    /// <returns>Search engine  name (slug)</returns>
    Task<string> GetSeNameAsync(int entityId, string entityName);

    /// <summary>
    /// Get slugs by entityName & storeId
    /// </summary>
    /// <param name="entityName">entityName</param>
    /// <param name="storeId">storeId</param>
    /// <returns>UrlRecords</returns>
    Task<IList<UrlRecord>> GetSlugsAsync(string entityName, int storeId);

}
