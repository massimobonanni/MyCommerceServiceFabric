using System.Threading.Tasks;

namespace MyCommerce.Common.Interfaces
{
    public interface IEntityRepository<TEntity, TKey> : IReadOnlyEntityRepository<TEntity,TKey > where TEntity : class 
    {
        Task<bool> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TKey id);
        Task<bool> SaveAsync();
    }

}
