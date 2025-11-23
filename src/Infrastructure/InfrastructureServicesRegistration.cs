using Application.Contracts.Identity;
using Application.Contracts.Infrastructure;
using Application.Models.Email;
using Application.Models.ImageManagement;
using Application.Models.Token;
using Application.Persistence;
using Infrastructure.MessageImplementation;
using Infrastructure.Repositiries;
using Infrastructure.Service.Auth;
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

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

            services.Configure<CloudIMGSettings>(configuration.GetSection(nameof(CloudIMGSettings)));

            services.Configure<EmailFluentSettings>(configuration.GetSection(nameof(EmailFluentSettings)));

            services.AddTransient<IEmailServices, EmailService>();
            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }
}
