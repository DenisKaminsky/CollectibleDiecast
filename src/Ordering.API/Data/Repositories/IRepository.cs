namespace CollectibleDiecast.Ordering.API.Data.Repositories;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}
