namespace CollectibleDiecast.Ordering.API.Data.Repositories;
public interface IRequestRepository
{
    Task<bool> ExistAsync(Guid id);

    Task CreateRequestForCommandAsync<T>(Guid id);
}
