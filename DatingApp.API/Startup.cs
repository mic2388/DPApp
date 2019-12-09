using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Helpers;
using AutoMapper;
using Microsoft.Extensions.Hosting;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       public void ConfigureProductionServices(IServiceCollection services)
        {
            
        //      services.AddDbContext<DataContext>(t 
        //       => t.UseSqlServer(Configuration.GetConnectionString("DefaultConnection1"),
        //    sqlServerOptionsAction: sqlOptions =>
        //     {
        //         sqlOptions.EnableRetryOnFailure(
        //         maxRetryCount: 10,
        //         maxRetryDelay: TimeSpan.FromSeconds(30),
        //         errorNumbersToAdd: null);
        //     }));
            
            services.AddDbContext<DataContext>(t=>{
               t.UseLazyLoadingProxies();
               t.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));  
            });


              ConfigureServices(services);

        }
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            //  services.AddDbContext<DataContext>(t 
            //   => t.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<DataContext>(t=>{
               t.UseLazyLoadingProxies();
               t.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));  
            });
            
            ConfigureServices(services);
        }
 

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(opt => {
                opt.SerializerSettings.ReferenceLoopHandling 
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            });
             

             ///** works for .net core 2.2 */
            // services.AddDbContext<DataContext>(t => t.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            // .AddJsonOptions(Opt=> {
            //     Opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            // });
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper(typeof(DatingRepository).Assembly);
            services.AddTransient<Seed>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                     new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // optional extra provider
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {

                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //  app.UseHsts();
            }
            
            //app.UseHttpsRedirection();
            app.UseRouting();

            //app.UseDeveloperExceptionPage();
            //seeder.SeedUsers();

            //order is important -- below
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors(t => t.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseDefaultFiles(); //this. looks for index.html file
            app.UseStaticFiles(); // this looks for wwwroot folder
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoint=>{
                    endpoint.MapControllers();
                    endpoint.MapFallbackToController("Index","Fallback");
            });
           

           // use for .net core 2.2
            // app.UseMvc(routes=> {
            //     routes.MapSpaFallbackRoute(
            //         name : "spa-fallback",
            //         defaults: new { controller ="Fallback", action = "Index"}
            //         );
            // });

        }
    }
}
