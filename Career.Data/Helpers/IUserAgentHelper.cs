namespace Career.Data.Helpers;

public interface IUserAgentHelper
{
    #region Methods

    /// <summary>
    /// Get a value indicating whether the request is made by mobile device
    /// </summary>
    /// <returns></returns>
    bool IsMobileDevice();

    #endregion
}
