namespace MarketKhoone.Entities.AuditableEntity
{
    public class AppShadowProperties
    {
        public string UserAgent { get; set; }
        public string UserIp { get; set; }
        public DateTime Now { get; set; }
        public long? UserId { get; set; }
    }
}
