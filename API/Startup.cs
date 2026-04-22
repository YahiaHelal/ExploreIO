using API.Extensions;
using API.Middleware;
using API.SignalR;
namespace API
{
    public class Startup
    {
        private IConfiguration _config{get;}
        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // Gets called by runtime, use it to add services
        // Takes care of creation/destruction of services classes (services scope -> Scoped, Transient, Singleton)
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config);
            // Controllers are instantiated via DI container.
            services.AddControllers();
            services.AddCors(); // Cross-Origin-Resource-Sharing policy
            services.AddIdentityServices(_config);
            services.AddSignalR();
        }

        // Gets called by runtime, use it to configure Http request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();         
            app.UseHttpsRedirection();

            app.UseRouting(); // router from endpoint to controller class

            app.UseCors(policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("https://localhost:4200"); // allow requests from your client
                //allow any header: authentication, ...
                //allow any method: post, get, put ... 
                //withOrigin: address of the client origin
            }); // CORS policy

            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}