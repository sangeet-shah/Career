using Career.Data.Domains.LandingPages;
using System.Threading.Tasks;
namespace Middleware.Web.Services.SummerJams;

/// <summary>
/// SummerJam service interface
/// </summary>
public interface ISummerJamService
{
    /// <summary>
    /// Insert summerJam
    /// </summary>
    /// <param name="summerJam">summerJam</param>
    Task InsertSummerJamAsync(SummerJam summerJam);
}
