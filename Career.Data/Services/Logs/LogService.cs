using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;

namespace Career.Data.Services.Logs;

public class LogService : ILogService
{
    private readonly IWebHelper _webHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;


    #region Ctor

    public LogService(IWebHelper webHelper,
        IHttpContextAccessor httpContextAccessor)
    {
        _webHelper = webHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inserts a log item
    /// </summary>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <returns>A log item</returns>
    public void InsertLog(string shortMessage, string fullMessage = "")
    {
        if (string.IsNullOrWhiteSpace(shortMessage))
            return;

        var headers = _httpContextAccessor.HttpContext?.Request?.Headers;

        Log.Write(
          LogEventLevel.Error,
          "Message: {ShortMessage} | Full: {FullMessage} | IP: {IP} | Url: {Url} | Referrer: {Referrer}",
          shortMessage,
          fullMessage,
          _webHelper.GetCurrentIpAddress(),
          _webHelper.GetThisPageUrl(true),
          _webHelper.GetUrlReferrer()
      );
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    public void Error(string message, Exception exception = null)
    {
        var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
        Log.Error(
            exception,
            "Message: {Message} | IP: {IP} | Url: {Url} | Referrer: {Referrer}",
            message,
            _webHelper.GetCurrentIpAddress(),
            _webHelper.GetThisPageUrl(true),
            _webHelper.GetUrlReferrer()
        );

    }

    #endregion

}
