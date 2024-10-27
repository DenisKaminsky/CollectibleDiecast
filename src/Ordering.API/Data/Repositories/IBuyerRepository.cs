namespace CollectibleDiecast.Ordering.API.Data.Repositories;

public interface IBuyerRepository : IRepository
{
    Buyer Add(Buyer buyer);
    Buyer Update(Buyer buyer);
    Task<Buyer> FindAsync(string BuyerIdentityGuid);
    Task<Buyer> FindByIdAsync(int id);
}

