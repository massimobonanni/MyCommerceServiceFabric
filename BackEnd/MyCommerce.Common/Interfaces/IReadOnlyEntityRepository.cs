using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyCommerce.Common.Interfaces
{
    public interface IReadOnlyEntityRepository<TEntity, TKey> : IDisposable where TEntity : class
    {
        IQueryable<TEntity> Query();
        Task<TEntity> GetSingleAsync(TKey key);
    }

}
