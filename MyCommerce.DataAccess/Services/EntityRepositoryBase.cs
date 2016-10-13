using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MyCommerce.Common.Interfaces;
using MyCommerce.DataAccess.Data;

namespace MyCommerce.DataAccess.Services
{
    public abstract class EntityRepositoryBase<TEntity, TKey> : IEntityRepository<TEntity, TKey> where TEntity : class
    {
        private readonly MyCommerceContext context;

        protected readonly bool TraceOff;

        protected EntityRepositoryBase(bool traceOff = false)
        {
            context = new MyCommerceContext();
            this.TraceOff = traceOff;
        }

        protected EntityRepositoryBase(string nameOrConnectionString, bool traceOff = false)
        {
            context = new MyCommerceContext(nameOrConnectionString);
            this.TraceOff = traceOff;
        }

        public DbSet<TEntity> Set => context.Set<TEntity>();

        public virtual IQueryable<TEntity> Query()
        {
            if (TraceOff)
                return Set.AsNoTracking();
            return Set;
        }

        public virtual Task<TEntity> GetSingleAsync(TKey key)
        {
            if (TraceOff)
                return GetSingleAsNoTrackingAsync(key);
            return Set.FindAsync(key);
        }

        public abstract Task<TEntity> GetSingleAsNoTrackingAsync(TKey key);

        public virtual Task<bool> AddAsync(TEntity entity)
        {
            Set.Add(entity);
            return Task.FromResult(true);
        }

        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity == null) return false;
            Set.Remove(entity);
            return true;
        }

        public virtual async Task<bool> SaveAsync()
        {
            return (await context.SaveChangesAsync()) > 0;
        }

        public virtual Task<bool> UpdateAsync(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(true);
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
