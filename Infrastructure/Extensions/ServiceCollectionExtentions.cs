using Application.Common.Interfaces.Repositories;
using Application.Interfaces.Auth;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Domain.Common.Security;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seeding;
using Infrastructure.Persistence.UnitOfWork;
using Infrastructure.Security;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Infrastructure.Services.CurrentUser;
using Infrastructure.Services.Email;
using Infrastructure.Services.Storage;
using Infrastructure.Services.Storage.Manager;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IVerificationService, VerificationService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, JwtService>();

            return services;
        }

        public static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProjectDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("AppString"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("AppString"))
                ));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

            return services;
        }

        public static async System.Threading.Tasks.Task SeedDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await DbInitializer.SeedAsync(context, passwordHasher);
        }

        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<EmailSettings>(opt => configuration.GetSection("Brevo"));

            services.AddSingleton(provider =>
            {
                var config = new brevo_csharp.Client.Configuration();
                var apiKey = configuration["Brevo:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                    throw new InvalidOperationException("Brevo API Key is not configured!");

                config.AddApiKey("api-key", apiKey);

                return new TransactionalEmailsApi(config);
            });

            services.AddScoped<IEmailService, BrevoEmailService>();

            return services;
        }

        public static IServiceCollection AddStorageService(this IServiceCollection services, string webRootPath)
        {
            services.AddSingleton(new LocalStorageService(webRootPath));
            services.AddSingleton<CloudinaryStorageService>();

            services.AddScoped<IStorageManager, StorageManager>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAgencyRepository, AgencyRepository>();
            services.AddScoped<IResponderRepository, ResponderRepository>();
            services.AddScoped<IIncidentRepository, IncidentRepository>();
            services.AddScoped<IIncidentResponderRepository, IncidentResponderRepository>();
            services.AddScoped<IIncidentLiveStreamRepository, IncidentLiveStreamRepository>();
            services.AddScoped<IIncidentMediaRepository, IncidentMediaRepository>();
            services.AddScoped<IIncidentLocationUpdateRepository, IncidentLocationUpdateRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IChatParticipantRepository, ChatParticipantRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            // services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            return services;
        }

    }
}
