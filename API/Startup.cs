using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

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
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });
            // Controllers are instantiated via DI container.
            services.AddControllers();
            services.AddCors(); // CORS policy
        }

        // Gets called by runtime, use it to configure Http request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // styled page to show exception for devs
            }
            
            app.UseHttpsRedirection(); // redirect to https if used http

            // browser -> weatherforecast -> weatherforecastController
            app.UseRouting(); // router from endpoint to controller class

            app.UseCors(policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"); // allow requests from your client
                //allow any header: authentication, ...
                //allow any method: post, get, put ... 
                //withOrigin: address of the client origin
            }); // CORS policy

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // map each endpoint to it's controller
            });
        }
    }
}