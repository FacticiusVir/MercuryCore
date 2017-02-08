using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keeper.DotMudCore
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Begins a property scope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns>
        /// An IDisposable that ends the property scope on dispose.
        /// </returns>
        /// <remarks>
        /// This only exists to prettify the way Serilog handles LogContext
        /// properties through the ILogger interface.
        /// </remarks>
        public static IDisposable BeginPropertyScope<T>(this ILogger logger, string propertyName, T value)
        {
            return logger.BeginScope(new Dictionary<string, object> { { propertyName, value } });
        }
    }
}
