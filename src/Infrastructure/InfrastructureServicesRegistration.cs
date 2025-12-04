using Application.Contracts.Identity;
using Application.Contracts.Infrastructure;
using Application.Interfaces;
using Application.Models.Email;
using Application.Models.ImageManagement;
using Application.Models.Token;
using Application.Persistence;
using Application.Services;
using Domain.Abstractions;
using Infrastructure.Excel;
using Infrastructure.MessageImplementation;
using Infrastructure.Parsers;
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));

            services.AddTransient<FileProcessorService>();

            // Mapeo del Parser (Lectura)
            services.AddTransient<IDataFileParser, SapDataFileParser>();
            services.AddTransient<IExcelDataWriter, ExcelDataWriter>();


            // Definición de una Factory Function (delegado)
            // Esta es la forma más limpia de manejar argumentos de constructor NO INYECTABLES.
            services.AddTransient<Func<string, IOutputWriter>>(sp =>
            {
                // Cuando FileProcessorService pida este Func, le devolvemos una función que:
                // 1. Recibe el filePath (string).
                // 2. Devuelve una nueva instancia de ExcelOutputWriter.
                return (filePath) => new ExcelOutputWriter(filePath);
            });

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

            services.Configure<CloudIMGSettings>(configuration.GetSection(nameof(CloudIMGSettings)));

            services.Configure<EmailFluentSettings>(configuration.GetSection(nameof(EmailFluentSettings)));

            services.AddTransient<IEmailServices, EmailService>();
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }
}
