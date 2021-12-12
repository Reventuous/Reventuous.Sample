namespace Reventuous.Sample.Application
{
    public static class Commands
    {
        public record CreateAccount(
            string accountId,
            string accountName,
            string accountEmail
        );

        public record CreditAccount(
            string accountId,
            decimal amount
        );
    }
}