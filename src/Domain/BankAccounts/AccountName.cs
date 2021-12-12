using System;

namespace Reventuous.Sample.Domain
{
    public record AccountName
    {
        public string Value { get; init; }
        public AccountName(string accountName)
        {
            if(String.IsNullOrEmpty(accountName))
                throw new ArgumentNullException(nameof(accountName));
            Value = accountName;
        }

        public static implicit operator string(AccountName accountName) => accountName.Value;
    }
}