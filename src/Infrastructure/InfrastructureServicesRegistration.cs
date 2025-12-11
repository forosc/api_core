using Application.Contracts.Identity;
using Application.Contracts.Infrastructure;
using Application.Interfaces;
using Application.Models.Email;
using Application.Models.ImageManagement;
using Application.Models.Token;
using Application.Persistence;
using Infrastructure.Excel;
using Infrastructure.MessageImplementation;
using Infrastructure.Parsers;
using Infrastructure.Processors;
using Infrastructure.Repositiries;
using Infrastructure.Service.Auth;
using Infrastructure.Writers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Data Access
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));

            // File Processing
            services.AddScoped<IDataFileParser, SapDataFileParser>();
            services.AddScoped<IExcelDataWriter, ExcelDataWriter>();
            services.AddScoped<IExcelGenerator<Domain.Model.POCO.BalanceData>,
                GenericExcelGenerator<Domain.Model.POCO.BalanceData>>();
            services.AddScoped<IExcelGenerator<string[]>,
                GenericExcelGenerator<string[]>>();

            services.AddScoped<IDataProcessor<Domain.Model.POCO.BalanceData>, DataProcessor>();

            // NO registrar el CommandHandler aquí - ya lo hace MediatR en Application layer
            // services.AddScoped<Application.UseCases.ProcessSapData.ProcessSapDataCommandHandler>();

            // Auth & Identity
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.AddTransient<IAuthService, AuthService>();

            // Email
            services.Configure<EmailFluentSettings>(configuration.GetSection(nameof(EmailFluentSettings)));
            services.AddTransient<IEmailServices, EmailService>();

            // Image Management (si se usa)
            services.Configure<CloudIMGSettings>(configuration.GetSection(nameof(CloudIMGSettings)));

            return services;
        }
    }
}
