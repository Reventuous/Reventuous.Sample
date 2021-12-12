using System.Threading;
using System.Threading.Tasks;
using Reventuous.Subscriptions;

using Reventuous.Sample.Domain;
using Reventuous.Sample.Infrastructure;
using static Reventuous.Sample.Domain.Events;
using static Reventuous.Sample.Application.Commands;

namespace Reventuous.Sample.Application.Reactions
{
    public class AccountReactor : IEventHandler
    {
        private ISendEmailService _emailService;
        public string SubscriptionId { get; }
        public AccountReactor(
            string subscriptionGroup,
            ISendEmailService emailService
        )
        {
            SubscriptionId = subscriptionGroup;
            _emailService = emailService;
        }

        public async Task HandleEvent(
            object @event,
#nullable enable
            string? position,
#nullable disable
            CancellationToken cancellationToken
        )
        {
            var result = @event switch
            {
                V1.AccountCreated created => 
                    _emailService.SendAccountCreatedEmail(
                        created.AccountEmail, 
                        created.AccountName
                    ),
                _ => Task.CompletedTask                     
            };

            await result;
        }
    }
}