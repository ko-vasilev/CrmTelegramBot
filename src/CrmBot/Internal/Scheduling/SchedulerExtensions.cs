using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace CrmBot.Internal.Scheduling
{
    public static class SchedulerExtensions
    {
        /// <summary>
        /// Registers a scheduler which will run scheduled tasks.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="unobservedTaskExceptionHandler">Handler which should handle unhandled exceptions.</param>
        /// <returns></returns>
        public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
        {
            return services.AddSingleton<IHostedService, SchedulerHostedService>(serviceProvider =>
            {
                var instance = new SchedulerHostedService(serviceProvider.GetServices<IScheduledTask>());
                instance.UnobservedTaskException += unobservedTaskExceptionHandler;
                return instance;
            });
        }
    }
}
