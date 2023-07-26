using MarketKhoone.Services.Contracts;
using System.Net;

namespace MarketKhoone.Services.Services
{
    public class SendSmsService : ISendSmsService
    {
        public void ProfileIsCreatedMessageToUserInWebsiteTwo(string phoneNumber)
        {
            var Client = new WebClient();
            var content = Client.DownloadString("http://panel.kavenegar.com/v1/6D6A694F346A6830695475787268476B6F4738576F33796842514D43384B52527867504B5130343871706F3D/verify/lookup.json?receptor=" + phoneNumber + "&token=" + phoneNumber + "&template=ProfileIsCreatedMessageToUserInWebsiteTwo");
        }

        public void LoginCodeForUserInWebsiteOne(string phoneNumber, string code)
        {
            var Client = new WebClient();
            var content = Client.DownloadString("http://panel.kavenegar.com/v1/6D6A694F346A6830695475787268476B6F4738576F33796842514D43384B52527867504B5130343871706F3D/verify/lookup.json?receptor=" + phoneNumber + "&token=" + code + "&template=LoginCodeForUserInWebsiteOne");
        }
    }
}
