using Career.Data.Domains.PaycorAPI;
using Career.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Middleware.Web.Services.PaycorAPI;

public interface IPaycorAPIService
{
    Task<IPagedList<PaycorAPIJobsResponse.JobRecord>> GetAllJobsAsync(IList<string> selectedStates = null,
        IList<string> selectedCities = null, IList<string> selectedJobCategories = null, int pageIndex = 0, int pageSize = 10000);
}
