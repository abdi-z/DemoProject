using backendAPI.IRepository;
using backendAPI.Models;
using backendAPI.Repository;
using backendAPI.Repository.Generic;
using backendAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace backendAPI.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) 
        {
            services.AddTransient<ILocationRepository, LocationRepository>();
           services.AddScoped<IUnitOfWork<DatabaseContextCla>, UnitOfWork<DatabaseContextCla>>();

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddDbContext<DatabaseContextCla>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("backendAPIContext"));
            });

            return services;
        }
    }
}
