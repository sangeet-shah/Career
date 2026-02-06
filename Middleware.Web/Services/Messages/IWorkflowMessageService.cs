using System.Threading.Tasks;

namespace Middleware.Web.Services.Messages;

public interface IWorkflowMessageService
{
    /// <summary>
    /// Send notification
    /// </summary>
    /// <param name="subject">subject</param>
    /// <param name="body">body</param>
    /// <param name="toEmailAddress">toEmailAddress</param>
    /// <param name="fromEmail">fromEmail</param>
    Task SendNotificationAsync(string subject, string body, string toEmailAddress, string fromEmail);
}
