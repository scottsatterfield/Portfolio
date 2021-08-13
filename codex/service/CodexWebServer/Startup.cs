using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CodexWebServer
{
    /// <summary>
    /// Start the ASP.NET web service.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initialize the Startup using the provided configuration.
        /// </summary>
        /// <param name="configuration">Web Service Configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime.
        /// This method adds services to the container.
        /// </summary>
        /// <param name="services">Collection of Service Descriptors</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "CodexWebServer", Version = "v1"});
            });
        }
        
        /// <summary>
        /// This method is used to configure the HTTP request pipeline.
        /// It gets called by the runtime.
        /// </summary>
        /// <param name="app">Mechanism to Configure the Request Pipeline</param>
        /// <param name="env">Web Hosting Environment Information</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodexWebServer v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}