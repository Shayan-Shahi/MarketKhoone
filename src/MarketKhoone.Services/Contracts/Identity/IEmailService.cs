namespace MarketKhoone.Services.Contracts.Identity
{
    public interface IEmailService
    {
        void SendEmail(string ReceiverEmail, string Subject, string Body);
    }
}
