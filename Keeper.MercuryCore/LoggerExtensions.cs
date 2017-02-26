using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Begins a property scope.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the property's value.
        /// </typeparam>
        /// <param name="logger">
        /// The value to which to apply the property.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <param name="value">
        /// The value of the property.
        /// </param>
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
