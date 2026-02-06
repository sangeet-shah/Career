using System;

namespace Career.Data.Services.Logs;

public interface ILogService
{
    #region Methods

    /// <summary>
    /// Inserts a log item
    /// </summary>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <returns>A log item</returns>
    void InsertLog(string shortMessage, string fullMessage = "");

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    void Error(string message, Exception exception = null);

    #endregion
}
