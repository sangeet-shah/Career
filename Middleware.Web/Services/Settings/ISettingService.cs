using Career.Data.Configuration;
using System;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Settings;

public interface ISettingService
{
    #region Methods

    /// <summary>
    /// Load settings
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    Task<T> LoadSettingAsync<T>(int storeId = 0) where T : ISettings, new();
    /// <summary>
    /// Load settings
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="storeId">Store identifier for which settings should be loaded</param>
    Task<ISettings> LoadSettingAsync(Type type, int storeId = 0);

    /// <summary>
    /// Get setting value by key
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    /// <returns>Setting value</returns>
    Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default(T),
        int storeId = 0, bool loadSharedValueIfNotFound = false);

    /// <summary>
    /// Get Setting value
    /// </summary>
    Task<string> GetSettingValueAsync(string name);

    /// <summary>
    /// Set setting value
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
    Task SetSettingAsync<T>(string key, T value, int storeId = 0, bool clearCache = true);

    #endregion
}
