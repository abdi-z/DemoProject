using Microsoft.EntityFrameworkCore;

namespace backendAPI.UnitOfWork
{
    public interface IUnitOfWork<out DatabaseContextCla> where DatabaseContextCla : DbContext, new()
    {
        //The following Property is going to hold the context object
        DatabaseContextCla Context { get; }
        //Start the database Transaction
        void CreateTransaction();
        //Commit the database Transaction
        void Commit();
        //Rollback the database Transaction
        void Rollback();
        //DbContext Class SaveChanges method
        void Save();
    }
}
        