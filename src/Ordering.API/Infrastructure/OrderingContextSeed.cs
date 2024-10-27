using CollectibleDiecast.Ordering.API.Data;
using CardType = CollectibleDiecast.Ordering.API.Data.Models.CardType;

namespace CollectibleDiecast.Ordering.API.Infrastructure;

public class OrderingContextSeed: IDbSeeder<OrderingContext>
{
    public async Task SeedAsync(OrderingContext context)
    {

        if (!context.CardTypes.Any())
        {
            context.CardTypes.AddRange(GetPredefinedCardTypes());

            await context.SaveChangesAsync();
        }

        await context.SaveChangesAsync();
    }

    private static IEnumerable<CardType> GetPredefinedCardTypes()
    {
        return Enumeration.GetAll<CardType>();
    }
}
