using System.Linq.Expressions;

namespace OmniFi_API.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Returns all elements of a specific Entity
        /// </summary>
        /// <param name="filter">Lambda expression to filter the entity query</param>
        /// <param name="includeProperties">List of the related properties that must be include with the entity</param>
        /// <param name="pageSize">Number of element per page</param>
        /// <param name="pageNumber">Number of the page</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties= null, int? pageSize = null, int pageNumber = 1);

        /// <summary>
        /// Return the first element of a specifi Entity filter
        /// </summary>
        /// <param name="filter">Lambda expression to filter the entity query</param>
        /// <param name="includeProperties">List of the related properties that must be include with the entity</param>
        /// <param name="tracked">Define if the enity must tracked or not</param>
        /// <returns></returns>
        public Task<T?> GetAsync(Expression<Func<T,bool>>? filter = null, string? includeProperties = null, bool tracked = true);
        public Task CreateAsync(T entity);
        public Task RemoveAsync(T entity);
        public Task SaveAsync();
    }
}
