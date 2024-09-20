namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBasketRepository BasketRepository { get; }
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> CompleteAsync();
    }
}
