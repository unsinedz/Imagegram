using Imagegram.Api.Extensions;
using Imagegram.Api.Mvc.ResultFilters;
using Imagegram.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Imagegram.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                config.Filters.Add(new ShortenedProblemDetailsResultFilter());
            }).AddNewtonsoftJson();

            services.Configure<ConnectionStringOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<FileStorageOptions>(Configuration.GetSection("FileStorage"));
            services.Configure<PostOptions>(Configuration.GetSection("Posts"));

            services.AddAutoMapper();

            services.AddTransient<IDbConnectionFactory, MsSqlConnectionFactory>();
            services.AddTransient<ICurrentUtcDateProvider, CurrentUtcDateProvider>();

            services.AddTransient<IImageConverter, ImageConverter>();
            services.AddTransient<IImageService, ImageService>();
            
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
