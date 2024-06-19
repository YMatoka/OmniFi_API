using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_API.Repository.Interfaces;
using System.Linq.Expressions;

namespace OmniFi_API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal DbSet<T> _dbSet;
        private readonly ApplicationDbContext _db;
        public const char PropertiesSeparator = ';';

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
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
                foreach (var property in includeProperties.Split(PropertiesSeparator,StringSplitOptions.RemoveEmptyEntries))
                {
                    _dbSet.Include(property);
                }
            }

            if(pageSize is not null)
            {
                _dbSet
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
                foreach(var property in includeProperties.Split(PropertiesSeparator, StringSplitOptions.RemoveEmptyEntries))
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
           await _db.SaveChangesAsync();
        }
    }
}
