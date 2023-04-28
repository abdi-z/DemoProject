using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using backendAPI.UnitOfWork;

namespace backendAPI.Repository.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public DatabaseContextCla _dbContext = null;
        private DbSet<T> table = null;
     
        private readonly IUnitOfWork<DatabaseContextCla> _unitOfWork;

        //public GenericRepository(IUnitOfWork<DatabaseContextCla> unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}
        public GenericRepository(DatabaseContextCla dbContext)
        {
            _unitOfWork = new UnitOfWork<DatabaseContextCla>(dbContext);
            _dbContext = dbContext;
            table = _dbContext.Set<T>();
        }
        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(List<T> entities)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {
            try
            {
                return table.AsQueryable();
            }
            catch
            {
                RollBack();
                return null;
            }
        }

        public Task<T> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(T entity)
        {

            try
            {
                await table.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                RollBack();
            }
            }

        public async Task InsertRangeAsync(List<T> entities)
        {
            try
            {
                await table.AddRangeAsync(entities);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                RollBack();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                table.Update(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                RollBack();
            }
        }

        private void RollBack()
        {
            throw new NotImplementedException();
        }

        public Task UpdateRangeAsync(List<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
