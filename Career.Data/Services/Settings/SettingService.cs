using Career.Data.Configuration;
using Career.Data.Data;
using Career.Data.Data.Caching;
using Career.Data.Domains;
using Career.Data.Domains.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Settings;

public class SettingService : ISettingService
{
    #region Fields

    private readonly IRepository<Setting> _settingRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public SettingService(IRepository<Setting> settingRepository,
        IStaticCacheManager staticCacheManager)
    {
        _settingRepository = settingRepository;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Nested classes

    /// <summary>
    /// Setting (for caching)
    /// </summary>
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

    /// <summary>
    /// Load settings
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    public async Task<T> LoadSettingAsync<T>(int storeId = 0) where T : ISettings, new()
    {
        return (T)await LoadSettingAsync(typeof(T), storeId);
    }

    /// <summary>
    /// Load settings
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    public async Task<ISettings> LoadSettingAsync(Type type, int storeId = 0)
    {
        var settings = Activator.CreateInstance(type);

        foreach (var prop in type.GetProperties())
        {
            // get properties we can read and write to
            if (!prop.CanRead || !prop.CanWrite)
                continue;

            var key = type.Name + "." + prop.Name;
            //load by store
            var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
            if (setting == null)
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                continue;

            if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                continue;

            var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

            //set property
            prop.SetValue(settings, value, null);
        }

        return settings as ISettings;
    }

    /// <summary>
    /// Get setting value by key
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    /// <returns>Setting value</returns>
    public async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default(T),
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

        //load shared value?
        if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
            setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

        return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
    }

    protected async Task<IDictionary<string, IList<SettingForCaching>>> GetAllSettingsCachedAsync()
    {
        //cache
        return await _staticCacheManager.GetAsync(CacheKeys.SettingsAllCacheKey, async () =>
        {
            //we use no tracking here for performance optimization
            //anyway records are loaded only for read-only operations
            var settings = await (from s in _settingRepository.Table
                                  orderby s.Name, s.StoreId
                                  select s).ToListAsync();

            var dictionary = new Dictionary<string, IList<SettingForCaching>>();
            foreach (var s in settings)
            {
                var resourceName = s.Name.ToLowerInvariant();
                var settingForCaching = new SettingForCaching
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Value,
                    StoreId = s.StoreId
                };
                if (!dictionary.ContainsKey(resourceName))
                {
                    //first setting
                    dictionary.Add(resourceName, new List<SettingForCaching>
                    {
                        settingForCaching
                    });
                }
                else
                {
                    //already added
                    //most probably it's the setting with the same name but for some certain store (storeId > 0)
                    dictionary[resourceName].Add(settingForCaching);
                }
            }

            return dictionary;
        });
    }

    /// <summary>
    /// Get setting by name
    /// </summary>
    /// <param name="name">name</param>
    /// <returns>Setting</returns>
    public async Task<Setting> GetSettingByNameAsync(string name)
    {
        var settings = await (from s in _settingRepository.Table
                              where s.Name.Contains(name)
                              select s).ToListAsync();

        return settings.FirstOrDefault();
    }

    /// <summary>
    /// Get Setting value
    /// </summary>
    public async Task<string> GetSettingValueAsync(string name)
    {
        return (await GetSettingByNameAsync(name))?.Value;
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    public async Task SetSettingAsync<T>(string key, T value, int storeId = 0, bool clearCache = true)
    {
        await SetSettingAsync(typeof(T), key, value, storeId, clearCache);
    }

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    protected async Task SetSettingAsync(Type type, string key, object value, int storeId = 0, bool clearCache = true)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        key = key.Trim().ToLowerInvariant();
        var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

        var settingForCaching = await GetSettingByNameAsync(key);
        if (settingForCaching != null)
        {
            //update
            var setting = await GetSettingByIdAsync(settingForCaching.Id);
            setting.Value = valueStr;
            await UpdateSettingAsync(setting);
        }
        else
        {
            //insert
            var setting = new Setting
            {
                Name = key,
                Value = valueStr,
                StoreId = storeId
            };
            await InsertSettingAsync(setting);
        }
    }

    /// <summary>
    /// Gets a setting by identifier
    /// </summary>
    /// <param name="settingId">Setting identifier</param>
    /// <returns>Setting</returns>
    public async Task<Setting> GetSettingByIdAsync(int settingId)
    {
        if (settingId == 0)
            return null;

        return await _settingRepository.Table.Where(x => x.Id == settingId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    public async Task InsertSettingAsync(Setting setting)
    {
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));

        await _settingRepository.InsertAsync(setting);
    }

    /// <summary>
    /// Updates a setting
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    public async Task UpdateSettingAsync(Setting setting)
    {
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));

        await _settingRepository.UpdateAsync(setting);
    }

    #endregion
}
