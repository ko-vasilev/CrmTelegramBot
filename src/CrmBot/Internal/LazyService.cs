using Microsoft.Extensions.DependencyInjection;
using System;

namespace CrmBot.Internal
{
    /// <summary>
    /// Allows support of resolving Lazy<T> services.
    /// </summary>
    public class LazyService<T> : Lazy<T> where T : class
    {
        public LazyService(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>())
        {
        }
    }
}
