// dotnet user-secrets set "Section:Key" "Value

namespace Reventuous.Sample.Infrastructure
{
    public class SendgridConfiguration
    {
        public string ApiKey { get; set; }
        public string SenderEmail { get; set;}
        public string SenderName { get; set; }
    }
}