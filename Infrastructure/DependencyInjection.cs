using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Connection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<ITrainingEventRepository, TrainingEventRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IWelderEvaluationRepository, WelderEvaluationRepository>();
            services.AddScoped<IProductionLineRepository, ProductionLineRepository>();

            services.AddScoped<IBlobStorageService, BlobStorageService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}