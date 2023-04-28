using System.Linq.Expressions;

namespace backendAPI.Repository.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T> GetAsync(Guid id);
        Task InsertAsync(T entity);
        Task InsertRangeAsync(List<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(List<T> entities);

        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(List<T> entities);
        IQueryable<T> FindAll(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
    }
}
