using Microsoft.EntityFrameworkCore;

namespace MarketKhoone.Common.EfToolkit
{
    public static class EfToolkit
    {
        public static void RegisterAllEntities(this ModelBuilder builder, Type type)
        {
            var entities = type.Assembly.GetTypes()
                .Where(x => x.BaseType == type);
            foreach (var entity in entities)
            {
                builder.Entity(entity);
            }
        }
    }
}
