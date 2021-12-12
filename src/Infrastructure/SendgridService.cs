using System;
using System.Net;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using FluentResults;

using Reventuous.Sample.Domain;

namespace Reventuous.Sample.Infrastructure
{
    public class SendGridService : ISendEmailService
    {
        private SendgridConfiguration _sendgridConfig;
        public SendGridService(
            IOptions<SendgridConfiguration> sendgridConfig
        )
        {
            if (string.IsNullOrWhiteSpace(sendgridConfig.Value.ApiKey))
                throw new ArgumentNullException(nameof(sendgridConfig.Value.ApiKey));

            if (string.IsNullOrWhiteSpace(sendgridConfig.Value.SenderEmail))
                throw new ArgumentNullException(nameof(sendgridConfig.Value.SenderEmail));

            if (string.IsNullOrWhiteSpace(sendgridConfig.Value.SenderName))
                throw new ArgumentNullException(nameof(sendgridConfig.Value.SenderName));

            _sendgridConfig = sendgridConfig.Value;
        }

        public async Task<Result> SendAccountCreatedEmail(
            string recipientEmail, 
            string recipientName
        )
        {
            var result = Result.Merge(
                Result.FailIf(string.IsNullOrWhiteSpace(recipientEmail), new RequiredError(nameof(recipientEmail))),
                Result.FailIf(string.IsNullOrWhiteSpace(recipientName), new RequiredError(nameof(recipientName)))
            );

            if (result.IsFailed)
                return result;

            var client = new SendGridClient(_sendgridConfig.ApiKey);
            var from = new EmailAddress(_sendgridConfig.SenderEmail, _sendgridConfig.SenderName);
            var subject = "Account Created";
            var to = new EmailAddress(recipientEmail, recipientName);
            var plainTextContent = $"Hi {recipientName}, a new bank account has been created";
            var htmlContent = $"Hi {recipientName}, a new bank account has been created";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return Result.Fail(new UnauthorizedError());
                }
                return Result.Fail(new UnknownError());
            }
        }
    }
}