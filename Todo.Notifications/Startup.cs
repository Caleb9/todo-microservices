using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.ServiceProvider;
using Todo.Notifications.Messages;
using Todo.Notifications.Notifications;

namespace Todo.Notifications
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
            services
                .AddSingleton(
                    new MessageBrokerConnectionString(
                        _configuration.GetConnectionString(MessageBrokerConnectionString.Name)))
                .AddRebus((config, diContainer) =>
                    config.Transport(t =>
                        t.UseRabbitMq(
                            diContainer.GetRequiredService<MessageBrokerConnectionString>(),
                            "items" /* Hardcoded but should be a setting */)))
                .AutoRegisterHandlersFromAssemblyOf<NewItemMessageHandler>();

            services
                .AddSingleton(
                    new RetryIntervalInSeconds(
                        _configuration.GetValue<int>(RetryIntervalInSeconds.ConfigurationKey)))
                .AddHostedService<MessageBrokerServicesSetup>();

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapHub<TodoListsHub>("/todo"); });
        }
    }
}