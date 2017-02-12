using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.DotMudCore
{
    public interface IServerBuilder
    {
        IServiceProvider Services { get; }

        SessionDelegate Build();

        IServerBuilder Use(Func<SessionDelegate, SessionDelegate> middleware);
    }

    public static class ServerBuilderExtensions
    {
        public static IServerBuilder UseMiddleware<T>(this IServerBuilder server)
            where T:IMiddleware
        {
            return server.Use(next => ActivatorUtilities.CreateInstance<T>(server.Services, next).Invoke);
        }

        public static void Run(this IServerBuilder server, SessionDelegate middleware)
        {
            server.Use(next => connection => middleware(connection));
        }
    }
}
