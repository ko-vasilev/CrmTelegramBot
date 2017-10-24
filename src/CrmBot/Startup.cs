using CrmBot.Bot;
using CrmBot.Bot.Commands;
using CrmBot.Internal;
using CrmBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;

namespace CrmBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddSingletonFromFile<AppSettings>(services, Configuration.GetSection("AppSettings"));
            AddSingletonFromFile<StorageSettings>(services, Configuration.GetSection("StorageSettings"));
            services.AddSingleton<TelegramBot>(serviceProvider =>
            {
                var telegramBotKey = serviceProvider.GetService<AppSettings>().TelegramBotKey;
                var bot = new TelegramBot(telegramBotKey, serviceProvider.GetRequiredService<TelegramBotMessageHandler>());
                bot.Activate();
                return bot;
            });
            services.AddTransient(serviceProvider =>
            {
                var storageSettings = serviceProvider.GetService<StorageSettings>();
                return new CloudStorageAccount(new StorageCredentials(storageSettings.AccountName, storageSettings.AccessKey), true);
            });
            services.AddTransient<AuthenticationStoreService>();
            services.AddTransient<AuthorizationService>();
            services.AddTransient<TelegramBotMessageHandler>();
            services.AddSingleton<CrmClientService>();
            services.AddTransient<CrmService>();
            services.AddTransient<ConversationService>();
            services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
            AddCommands(services);

            services.AddDataProtection();
            services.AddMemoryCache();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // Initialize the telegram bot
            app.ApplicationServices.GetService<TelegramBot>();
        }

        private static void AddSingletonFromFile<TOptions>(
            IServiceCollection services,
            IConfiguration configuration)
            where TOptions : class, new()
        {
            //POCO is created with actual values
            TOptions options = configuration.Get<TOptions>();
            services.AddSingleton(options);
        }

        private void AddCommands(IServiceCollection services)
        {
            services.AddTransient<GetAuthorizationUrlCommand>();
            services.AddTransient<NotifySuccessfulConnectionCommand>();
            services.AddTransient<UpdateDailyReportCommand>();
            services.AddTransient<NotifyUnknownCommandCommand>();
            services.AddTransient<GetDayJobProgressCommand>();
        }
    }
}
