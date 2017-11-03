﻿using CrmBot.Bot;
using CrmBot.Bot.Commands;
using CrmBot.DataAccess;
using CrmBot.DataAccess.Services;
using CrmBot.Internal;
using CrmBot.Internal.Scheduling;
using CrmBot.PeriodicTasks;
using CrmBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<TelegramBot>(serviceProvider =>
            {
                var telegramBotKey = serviceProvider.GetService<AppSettings>().TelegramBotKey;
                var bot = new TelegramBot(telegramBotKey, serviceProvider.GetRequiredService<TelegramBotMessageHandler>());
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
            services.AddMvc();

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
                // @TODO: add error handling logic
                Console.Write(args.Exception.Message);
                args.SetObserved();
            });
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
            services.AddTransient<SubscribeDailyReportNotificationsCommand>();
        }
    }
}
