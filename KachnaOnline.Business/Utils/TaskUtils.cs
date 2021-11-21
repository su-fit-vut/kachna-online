// TaskUtils.cs
// Author: Ondřej Ondryáš

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.Business.Utils
{
    public static class TaskUtils
    {
        /// <summary>
        /// Creates an async scope from the given <see cref="IServiceProvider"/> and starts a <see cref="Task"/>
        /// in the background. Exceptions from the task call are caught and logged.
        /// </summary>
        /// <param name="serviceProvider">An <see cref="IServiceProvider"/>.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="action">A <see cref="Func{T1,T2,TResult}"/> that accepts a <see cref="IServiceProvider"/>
        /// and an <see cref="ILogger{T}"/> and returns a <see cref="Task"/> representing the asynchronous
        /// operation to run in the background.</param>
        /// <typeparam name="T">The type whose name is used for the logger category name.</typeparam>
        public static void FireAndForget<T>(IServiceProvider serviceProvider,
            ILogger<T> logger, Func<IServiceProvider, ILogger<T>, Task> action)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    await action(scope.ServiceProvider, logger);
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "An exception occurred when performing a background task.");
                }
            }).ConfigureAwait(false);
        }
    }
}
