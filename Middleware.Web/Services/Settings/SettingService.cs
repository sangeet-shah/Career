using Career.Data.Configuration;
using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains;
using Career.Data.Domains.Common;
using Dapper;
using Middleware.Web.Data;
using Middleware.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Settings;

public class SettingService : ISettingService
{
    private const string SettingTable = "Setting";

    #region Fields

    private readonly DbConnectionFactory _db;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public SettingService(DbConnectionFactory db,
        IStaticCacheManager staticCacheManager)
    {
        _db = db;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Nested classes

    [Serializable]
    public class SettingForCaching
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int StoreId { get; set; }
    }

    #endregion

    #region Methods

    public async Task<T> LoadSettingAsync<T>(int storeId = 0) where T : ISettings, new()
    {
        return (T)await LoadSettingAsync(typeof(T), storeId);
    }

    public async Task<ISettings> LoadSettingAsync(Type type, int storeId = 0)
    {
        var settings = Activator.CreateInstance(type);

        foreach (var prop in type.GetProperties())
        {
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var key = type.Name + "." + prop.Name;
            var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
            if (setting == null)
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                continue;

            var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);
            prop.SetValue(settings, value, null);
        }

        return settings as ISettings;
    }

    public async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default,
        int storeId = 0, bool loadSharedValueIfNotFound = false)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        var settings = await GetAllSettingsCachedAsync();
        key = key.Trim().ToLowerInvariant();
        if (!settings.ContainsKey(key))
            return defaultValue;

        var settingsByKey = settings[key];
        var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

        if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

        return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
    }

    protected async Task<IDictionary<string, IList<SettingForCaching>>> GetAllSettingsCachedAsync()
    {
        return await _staticCacheManager.GetAsync(CacheKeys.SettingsAllCacheKey, async () =>
        {
            using var conn = _db.CreateNop();
            var settings = (await conn.QueryAsync<SettingForCaching>(
                $"SELECT Id, Name, Value, StoreId FROM [{SettingTable}] ORDER BY Name, StoreId")).ToList();

            var dictionary = new Dictionary<string, IList<SettingForCaching>>();
            foreach (var s in settings)
            {
                var resourceName = s.Name.ToLowerInvariant();
                var settingForCaching = new SettingForCaching { Id = s.Id, Name = s.Name, Value = s.Value, StoreId = s.StoreId };
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new List<SettingForCaching> { settingForCaching });
                else
                    dictionary[resourceName].Add(settingForCaching);
            }

            return dictionary;
        });
    }

    public async Task<Setting> GetSettingByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        using var conn = _db.CreateNop();
        var sql = $"SELECT Id, Name, Value, StoreId FROM [{SettingTable}] WHERE Name LIKE @Name";
        return (await conn.QueryAsync<Setting>(sql, new { Name = "%" + name + "%" })).FirstOrDefault();
    }

    public async Task<string> GetSettingValueAsync(string name)
    {
        return (await GetSettingByNameAsync(name))?.Value;
    }

    public async Task SetSettingAsync<T>(string key, T value, int storeId = 0, bool clearCache = true)
    {
        await SetSettingAsync(typeof(T), key, value, storeId, clearCache);
    }

    protected async Task SetSettingAsync(Type type, string key, object value, int storeId = 0, bool clearCache = true)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        key = key.Trim().ToLowerInvariant();
        var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

        var settingForCaching = await GetSettingByNameAsync(key);
        if (settingForCaching != null)
        {
            var setting = await GetSettingByIdAsync(settingForCaching.Id);
            setting.Value = valueStr;
            await UpdateSettingAsync(setting);
        }
        else
        {
            await InsertSettingAsync(new Setting { Name = key, Value = valueStr, StoreId = storeId });
        }
    }

    public async Task<Setting> GetSettingByIdAsync(int settingId)
    {
        if (settingId == 0)
            return null;
        using var conn = _db.CreateNop();
        var sql = $"SELECT Id, Name, Value, StoreId FROM [{SettingTable}] WHERE Id = @Id";
        return await conn.QueryFirstOrDefaultAsync<Setting>(sql, new { Id = settingId });
    }

    public async Task InsertSettingAsync(Setting setting)
    {
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));

        using var conn = _db.CreateNop();
        var sql = $"INSERT INTO [{SettingTable}] (Name, Value, StoreId) VALUES (@Name, @Value, @StoreId); SELECT CAST(SCOPE_IDENTITY() AS INT);";
        setting.Id = await conn.ExecuteScalarAsync<int>(sql, setting);
    }

    public async Task UpdateSettingAsync(Setting setting)
    {
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));

        using var conn = _db.CreateNop();
        var sql = $"UPDATE [{SettingTable}] SET Name = @Name, Value = @Value, StoreId = @StoreId WHERE Id = @Id";
        await conn.ExecuteAsync(sql, setting);
    }

    #endregion
}
