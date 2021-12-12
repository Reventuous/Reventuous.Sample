namespace Reventuous.Sample.Domain
{
    public static class Events
    {
        public static class V1
        {
            public record AccountCreated(
                string AccountId,
                string AccountName,
                string AccountEmail
            );
            public record AccountCredited(
                decimal Amount
            );
            public record AccountDebited(
                decimal Amount
            );
        }
    }
}