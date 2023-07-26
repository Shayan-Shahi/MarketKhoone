using Microsoft.AspNetCore.Identity;

namespace MarketKhoone.ViewModels.Identity.Settings;

public class SiteSettings
{
    public string UsersAvatarsFolder { get; set; }
    public string UserDefaultAvatar { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
    public bool EnableEmailConfirmation { get; set; }
    public TimeSpan EmailConfirmationTokenProviderLifespan { get; set; }
    public PasswordOptions PasswordOptions { get; set; }
    public LockoutOptions LockoutOptions { get; set; }
    public CookieOptions CookieOptions { get; set; }

}
