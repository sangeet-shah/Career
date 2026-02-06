using Career.Data.Data;
using Career.Data.Domains.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Career.Data.Services.Messages;

public class WorkflowMessageService : IWorkflowMessageService
{
    #region Fields

    private readonly IRepository<QueuedEmail> _queuedEmailRepository;
    private readonly IRepository<EmailAccount> _emailAccountRepository;

    #endregion

    #region Ctor

    public WorkflowMessageService(IRepository<QueuedEmail> queuedEmailRepository,
        IRepository<EmailAccount> emailAccountRepository)
    {
        _queuedEmailRepository = queuedEmailRepository;
        _emailAccountRepository = emailAccountRepository;
    }

    #endregion

    #region Methods  

    /// <summary>
    /// Send notification
    /// </summary>
    /// <param name="subject">subject</param>
    /// <param name="body">body</param>
    /// <param name="toEmailAddress">toEmailAddress</param>
    /// <param name="fromEmail">fromEmail</param>
    public async Task SendNotificationAsync(string subject, string body, string toEmailAddress, string fromEmail)
    {
        // check if to emailaddress and fromemail should be required to trigger email
        if (string.IsNullOrEmpty(toEmailAddress) || string.IsNullOrEmpty(fromEmail))
            return;

        //email account            
        foreach (var toEmail in toEmailAddress.Split(','))
        {
            var email = new QueuedEmail
            {
                From = fromEmail,
                FromName = fromEmail,
                To = toEmail,
                ToName = toEmail,
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = _emailAccountRepository.Table.Select(e => e.Id).FirstOrDefault(),
            };

            await _queuedEmailRepository.InsertAsync(email);
        }
    }

    #endregion
}
