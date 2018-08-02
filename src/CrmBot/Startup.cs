using CrmBot.Bot;
using CrmBot.Bot.Commands;
using CrmBot.DataAccess;
using CrmBot.DataAccess.Services;
using CrmBot.Internal;
using CrmBot.Internal.Scheduling;
using CrmBot.PeriodicTasks;
using CrmBot.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace CrmBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddSingletonFromFile<AppSettings>(services, Configuration.GetSection("AppSettings"));
            services.AddSingleton<TelegramBot>(serviceProvider =>
            {
                var appSettings = serviceProvider.GetService<AppSettings>();
                var telegramBotKey = appSettings.TelegramBotKey;
                WebProxy proxy = null;
                if (!string.IsNullOrEmpty(appSettings.TelegramProxyAddress) && appSettings.TelegramProxyPort.HasValue)
                {
                    proxy = new WebProxy(appSettings.TelegramProxyAddress, appSettings.TelegramProxyPort.Value);
                    proxy.BypassProxyOnLocal = false;
                }
                var bot = new TelegramBot(telegramBotKey, serviceProvider.GetRequiredService<TelegramBotMessageHandler>(), proxy);
                bot.Activate();
                return bot;
            });
            services.AddTransient<AuthorizationService>();
            services.AddTransient<TelegramBotMessageHandler>();
            services.AddSingleton<CrmClientService>();
            services.AddTransient<CrmService>();
            services.AddTransient<ConversationService>();
            services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
            AddCommands(services);

            services.AddDataProtection();
            services.AddMemoryCache();
            services.AddMvc(); // Fix compilation error if referencing NetStandard2.0 assemblies

            string appInsightsKey = Configuration.GetSection("ApplicationInsights")["InstrumentationKey"];
            if (!string.IsNullOrEmpty(appInsightsKey))
            {
                services.AddApplicationInsightsTelemetry(appInsightsKey);
            }
            services.AddScoped<TelemetryClient>();

            string connectionString = Configuration["connectionString"];
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);
            services.AddSingleton<IAppUnitOfWorkFactory, AppUnitOfWorkFactory>(serviceProvider =>
            {
                var options = serviceProvider.GetService<DbContextOptions<DatabaseContext>>();
                return new AppUnitOfWorkFactory(options);
            });
            services.AddTransient<TelegramChatService>();
            services.AddTransient<UserService>();
            services.AddTransient<NotificationSubscriptionService>();

            // Register scheduled tasks.
            services.AddSingleton<IScheduledTask, CheckSubmittedDailyReportsTask>();
            services.AddScheduler((sender, args) =>
            {
                new TelemetryClient().TrackException(args.Exception);
                args.SetObserved();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Error);

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
            services.AddTransient<SubscribeDailyReportNotificationsCommand>();
            services.AddTransient<HelpCommand>();
        }
    }
}
