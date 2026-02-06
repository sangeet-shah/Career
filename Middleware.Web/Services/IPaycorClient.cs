namespace Middleware.Web.Services;

public interface IPaycorClient
{
    Task<string?> GetEmployeesWithHoursAndPunchesAsync(CancellationToken ct);
}

