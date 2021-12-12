using System;
using Reventuous;
using Reventuous.Sample.Domain;
using static Reventuous.Sample.Application.Commands;

namespace Reventuous.Sample.Application
{
    public class AccountService : ApplicationService<Account, AccountState, AccountId>
    {
        public AccountService(
            IAggregateStore store
        ) : base(store)
        {
            OnAny<CreateAccount>(
                cmd => new AccountId(cmd.accountId),
                (account, cmd) => account.Create(
                    new AccountId(cmd.accountId),
                    new AccountName(cmd.accountName),
                    new AccountEmail(cmd.accountEmail)
                )
            );

            OnAny<CreditAccount>(
                cmd => new AccountId(cmd.accountId),
                (account, cmd) => account.Credit(cmd.amount)
            );
        }
    }
}