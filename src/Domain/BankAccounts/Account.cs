using System;
using System.Threading.Tasks;
using Reventuous;
using static Reventuous.Sample.Domain.Events;

namespace Reventuous.Sample.Domain
{
    public record AccountId(string Value): AggregateId(Value);

    public class Account: Aggregate<AccountState, AccountId>
    {
        public void Create(AccountId accountId, AccountName accountName, AccountEmail accountEmail)
        {
            EnsureDoesntExist();
            Apply(new V1.AccountCreated(accountId, accountName, accountEmail));
        }

        public void Credit(decimal amount)
        {
            Apply(new V1.AccountCredited(amount));
        }
    }

    public record AccountState : AggregateState<AccountState, AccountId>
    {
        public AccountName Name { get; init; }
        public AccountEmail Email { get; init; }
        public decimal Balance { get; init; } 

        public override AccountState When(object @event)
            => @event switch {
                V1.AccountCreated created => this with {
                    Id = new AccountId(created.AccountId),
                    Name = new AccountName(created.AccountName),
                    Email = new AccountEmail(created.AccountEmail)
                },
                V1.AccountCredited credited => this with {
                    Balance = Balance + credited.Amount
                },
                _ => this
            };
    }
}