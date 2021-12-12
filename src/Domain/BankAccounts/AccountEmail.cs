using System;

namespace Reventuous.Sample.Domain
{
    public record AccountEmail
    {
        public string Value { get; init; }
        public AccountEmail(string accountEmail)
        {
            if(String.IsNullOrEmpty(accountEmail))
                throw new ArgumentNullException(nameof(accountEmail));
            Value = accountEmail;
        }

        public static implicit operator string(AccountEmail accountEmail) => accountEmail.Value;
    }
}