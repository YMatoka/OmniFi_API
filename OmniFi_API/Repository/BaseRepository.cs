using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Linq.Expressions;

namespace OmniFi_API.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> _dbSet;
        protected readonly ApplicationDbContext db;

        public BaseRepository(ApplicationDbContext db)
        {
            this.db = db;
           
            _dbSet = this.db.Set<T>();
        }
        public virtual async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, int? pageSize = null, int pageNumber = 1)
        {
            IQueryable<T> query = _dbSet;

            if(filter is not null)
            {
                query = query.Where(filter);
            }

            if(includeProperties is not null)
            {
                foreach (var property in includeProperties.Split(
                    EntityUtilities.PropertiesSeperator,StringSplitOptions.RemoveEmptyEntries))
                {
                    await query.Include(property).LoadAsync();
                }
            }

            if(pageSize is not null)
            {
                query
                    .Skip((int)pageSize * (pageNumber - 1))
                    .Take((int)pageSize);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query = _dbSet;
            
            if(filter is not null)
            {
                query = query.Where(filter);
            }

            if(includeProperties is not null)
            {
                foreach(var property in includeProperties.Split(EntityUtilities.PropertiesSeperator, StringSplitOptions.RemoveEmptyEntries))
                {
                    _dbSet.Include(property);
                }
            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task SaveAsync()
        {
           await db.SaveChangesAsync();
        }
    }
}
