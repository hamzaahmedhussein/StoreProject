using Core.Interfaces;
using System.Collections;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IBasketRepository _basketRepository;
        private Hashtable _repositories;

        public UnitOfWork(ApplicationDbContext context, IBasketRepository basketRepository)
        {
            _context = context;
            _basketRepository = basketRepository;
        }

        public IBasketRepository BasketRepository => _basketRepository;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Hashtable();
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IGenericRepository<TEntity>)_repositories[type];
        }
    }
}
