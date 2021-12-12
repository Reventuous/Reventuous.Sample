using System.Threading.Tasks;
using FluentResults;

namespace Reventuous.Sample.Domain
{
    public interface ISendEmailService
    {
        Task<Result> SendAccountCreatedEmail(
            string recipientEmail, 
            string recipientName
        );
    }
}