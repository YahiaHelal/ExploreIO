using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<PresenceTracker>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddDbContext<DataContext>(options =>
            {
                var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                
                string connStr;

                if (string.IsNullOrEmpty(databaseUrl))
                {
                    // local dev
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    var uri = new Uri(databaseUrl);

                    var userInfo = uri.UserInfo.Split(':');
                    var user = userInfo[0];
                    var password = userInfo[1];

                    connStr = $"Host={uri.Host};" +
                            $"Port={uri.Port};" +
                            $"Database={uri.AbsolutePath.Trim('/')};" +
                            $"Username={user};" +
                            $"Password={password};" +
                            $"SSL Mode=Require;" +
                            $"Trust Server Certificate=true";
                }

                options.UseNpgsql(connStr);
            });
            return services;
        }
    }
}

