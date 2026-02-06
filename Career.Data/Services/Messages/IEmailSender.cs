using Career.Data.Domains.Career;
using Career.Data.Domains.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Career.Data.Services.Messages;

/// <summary>
/// Email sender
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email
    /// </summary>
    /// <param name="emailAccount">Email account to use</param>
    /// <param name="subject">Subject</param>
    /// <param name="body">Body</param>
    /// <param name="fromAddress">From address</param>
    /// <param name="fromName">From display name</param>
    /// <param name="toAddress">To address</param>
    /// <param name="toName">To display name</param>
    /// <param name="replyTo">ReplyTo address</param>
    /// <param name="replyToName">ReplyTo display name</param>
    /// <param name="bcc">BCC addresses list</param>
    /// <param name="cc">CC addresses list</param>
    /// <param name="attachmentFilePath">Attachment file path</param>
    /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
    /// <param name="attachedDownloads">Attachment download ID (another attachment)</param>        
    Task SendEmailAsync(EmailAccount emailAccount, string subject, string body,
        string fromAddress, string fromName, string toAddress, string toName,
        string replyTo = null, string replyToName = null,
        IList<string> bcc = null, IList<string> cc = null,
        string attachmentFilePath = null, string attachmentFileName = null,
        IList<Download> attachedDownloads = null);
}
