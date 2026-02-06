using Career.Data.Domains.LandingPages;
using System.Threading.Tasks;
namespace Career.Data.Services.SummerJams;

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
