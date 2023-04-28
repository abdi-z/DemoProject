using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;


namespace backendAPI.UnitOfWork
{
    public class UnitOfWork<DatabaseContextCla> : IUnitOfWork<DatabaseContextCla>, IDisposable where DatabaseContextCla : DbContext, new()
    {
        private bool _disposed;
        private string _errorMessage = string.Empty;
        //The following Object is going to hold the Transaction Object
        private IDbContextTransaction _objTran;

        //Using the Constructor we are initializing the Context Property which is declared in the IUnitOfWork Interface
        //This is nothing but we are storing the DBContext (EmployeeDBContext) object in Context Property
        public UnitOfWork(DatabaseContextCla dbContext)
        {
            Context = dbContext;
        }
        //The Dispose() method is used to free unmanaged resources like files, 
        //database connections etc. at any time.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //The Context property will return the DBContext object i.e. (EmployeeDBContext) object
        //This Property is declared inside the Parent Interface and Initialized through the Constructor
        public DatabaseContextCla Context { get; }
        //The CreateTransaction() method will create a database Transaction so that we can do database operations
        //by applying do everything and do nothing principle
        public void CreateTransaction()
        {
            //It will Begin the transaction on the underlying store connection
            _objTran = Context.Database.BeginTransaction();
        }
        //If all the Transactions are completed successfully then we need to call this Commit() 
        //method to Save the changes permanently in the database
        public void Commit()
        {
            //Commits the underlying store transaction
            _objTran.Commit();
        }
        //If at least one of the Transaction is Failed then we need to call this Rollback() 
        //method to Rollback the database changes to its previous state
        public void Rollback()
        {
            //Rolls back the underlying store transaction
            _objTran.Rollback();
            //The Dispose Method will clean up this transaction object and ensures Entity Framework
            //is no longer using that transaction.
            _objTran.Dispose();
        }
        //The Save() Method Implement DbContext Class SaveChanges method 
        //So whenever we do a transaction we need to call this Save() method 
        //so that it will make the changes in the database permanently
        public void Save()
        {
            try
            {
                //Calling DbContext Class SaveChanges method 
                Context.SaveChanges();
            }
            catch (Exception dbEx)
            {
                throw new Exception(_errorMessage, dbEx);
            }
        }
        //Disposing of the Context Object
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    Context.Dispose();
            _disposed = true;
        }
    }
}
