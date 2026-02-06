using Microsoft.Data.SqlClient;

namespace Middleware.Web.Data;

public sealed class DbConnectionFactory
{
    private readonly IConfiguration _config;

    public DbConnectionFactory(IConfiguration config) => _config = config;

    public SqlConnection CreateEBridge()
        => new SqlConnection(_config.GetConnectionString("EBridge"));

    public SqlConnection CreateNop()
        => new SqlConnection(_config.GetConnectionString("Nop"));
}
