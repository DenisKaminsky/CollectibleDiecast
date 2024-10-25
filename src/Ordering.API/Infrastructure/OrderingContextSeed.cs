namespace CollectibleDiecast.Ordering.API.Infrastructure;

using CollectibleDiecast.Ordering.Domain.AggregatesModel.BuyerAggregate;

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
