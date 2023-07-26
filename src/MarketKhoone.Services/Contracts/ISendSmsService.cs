namespace MarketKhoone.Services.Contracts
{
    public interface ISendSmsService
    {
        #region BaseClass

        void LoginCodeForUserInWebsiteOne(string phoneNumber, string code);
        void ProfileIsCreatedMessageToUserInWebsiteTwo(string phoneNumber);

        #endregion
    }
}
