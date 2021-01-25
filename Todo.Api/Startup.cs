using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Todo.Api.Infrastructure;

namespace Todo.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options =>
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Todo.Api", Version = "v1"}));

            services
                .AddDataLayerDependencies(_configuration)
                .AddMessageBrokerDependencies(_configuration)
                .AddMessageBrokerBackgroundServices(_configuration)
                .AddApplicationServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /* By default Swagger is available in Development environment only.
             * Because I don't have a client implemented, for this example I also run it in
             * "Production". */
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo.Api v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}