using Middleware.Web.Data;
using Middleware.Web.Domains.Messages;
using Dapper;
using Middleware.Web.Data;
using System;
using System.Threading.Tasks;

namespace Middleware.Web.Services.Messages;

public class WorkflowMessageService : IWorkflowMessageService
{
    private const string QueuedEmailTable = "QueuedEmail";
    private const string EmailAccountTable = "EmailAccount";

    private readonly DbConnectionFactory _db;

    public WorkflowMessageService(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task SendNotificationAsync(string subject, string body, string toEmailAddress, string fromEmail)
    {
        if (string.IsNullOrEmpty(toEmailAddress) || string.IsNullOrEmpty(fromEmail))
            return;

        int emailAccountId = 0;
        using (var conn = _db.CreateNop())
        {
            emailAccountId = await conn.ExecuteScalarAsync<int>($"SELECT TOP 1 Id FROM [{EmailAccountTable}]");
        }

        foreach (var toEmail in toEmailAddress.Split(','))
        {
            var email = new QueuedEmail
            {
                From = fromEmail,
                FromName = fromEmail,
                To = toEmail.Trim(),
                ToName = toEmail.Trim(),
                Subject = subject,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccountId
            };

            using var conn2 = _db.CreateNop();
            var sql = $@"INSERT INTO [{QueuedEmailTable}] (PriorityId, [From], FromName, [To], ToName, ReplyTo, ReplyToName, CC, Bcc, Subject, Body, CreatedOnUtc, EmailAccountId, SentTries)
VALUES (0, @From, @FromName, @To, @ToName, @ReplyTo, @ReplyToName, @CC, @Bcc, @Subject, @Body, @CreatedOnUtc, @EmailAccountId, 0)";
            await conn2.ExecuteAsync(sql, email);
        }
    }
}
