using Middleware.Web.Data;
using Middleware.Web.Domains.Messages;
using Dapper;
using Middleware.Web.Data;
using Middleware.Web.Helpers;
using System;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Messages;

public class NewsLetterSubscriptionService : INewsLetterSubscriptionService
{
    private const string NewsLetterSubscriptionTable = "NewsLetterSubscription";

    private readonly DbConnectionFactory _db;

    public NewsLetterSubscriptionService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        if (newsLetterSubscription == null)
            throw new ArgumentNullException(nameof(newsLetterSubscription));

        newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

        using var conn = _db.CreateNop();
        var sql = $@"INSERT INTO [{NewsLetterSubscriptionTable}] (NewsLetterSubscriptionGuid, Email, Active, StoreId)
VALUES (@NewsLetterSubscriptionGuid, @Email, @Active, @StoreId);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
        newsLetterSubscription.Id = await conn.ExecuteScalarAsync<int>(sql, newsLetterSubscription);
    }

    public async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByEmailAndStoreIdAsync(string email, int storeId)
    {
        if (!CommonHelper.IsValidEmail(email))
            return null;

        using var conn = _db.CreateNop();
        var sql = $"SELECT * FROM [{NewsLetterSubscriptionTable}] WHERE Email = @Email AND StoreId = @StoreId ORDER BY Id";
        return await conn.QueryFirstOrDefaultAsync<NewsLetterSubscription>(sql, new { Email = email.Trim(), StoreId = storeId });
    }

    public async Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        if (newsLetterSubscription == null)
            throw new ArgumentNullException(nameof(newsLetterSubscription));

        newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

        using var conn = _db.CreateNop();
        var sql = $@"UPDATE [{NewsLetterSubscriptionTable}] SET NewsLetterSubscriptionGuid = @NewsLetterSubscriptionGuid, Email = @Email, Active = @Active, StoreId = @StoreId WHERE Id = @Id";
        await conn.ExecuteAsync(sql, newsLetterSubscription);
    }
}
