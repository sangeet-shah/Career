using Dapper;
using Middleware.Web.Options;
using Microsoft.Extensions.Options;

namespace Middleware.Web.Data;

public sealed class NopSettingsRepository : INopSettingsRepository
{
    private readonly DbConnectionFactory _db;
    private readonly MiddlewareOptions _opt;

    public NopSettingsRepository(DbConnectionFactory db, IOptions<MiddlewareOptions> opt)
    {
        _db = db;
        _opt = opt.Value;
    }

    public async Task<string?> GetSettingValueAsync(string name, CancellationToken ct)
    {
        const string sql = @"
            SELECT TOP 1 [Value]
            FROM dbo.[Setting]
            WHERE [Name] = @Name
            AND [StoreId] = 0
            ORDER BY [Id] DESC;";

        using var con = _db.CreateNop();
        return await con.QuerySingleOrDefaultAsync<string?>(
            new CommandDefinition(sql, new { Name = name }, commandTimeout: _opt.CommandTimeoutSeconds, cancellationToken: ct));
    }

    public async Task SetSettingValueAsync(string name, string value, CancellationToken ct)
    {
        const string sql = @"
            IF EXISTS(SELECT 1 FROM dbo.[Setting] WHERE [Name] = @Name)
            BEGIN
                UPDATE dbo.[Setting]
                SET [Value] = @Value
                WHERE [Name] = @Name;
            END
            ELSE
            BEGIN
                INSERT INTO dbo.[Setting] ([Name],[Value],[StoreId])
                VALUES (@Name, @Value, 0);
            END";

        using var con = _db.CreateNop();
        await con.ExecuteAsync(new CommandDefinition(
            sql,
            new { Name = name, Value = value },
            commandTimeout: _opt.CommandTimeoutSeconds,
            cancellationToken: ct));
    }

}
