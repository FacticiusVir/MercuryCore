﻿using Keeper.MercuryCore.Session;
using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Middleware
{
    public class MotdMiddleware
        : IMiddleware
    {
        private readonly MotdOptions options;

        public MotdMiddleware(IOptions<MotdOptions> options)
        {
            this.options = options.Value;
        }

        public Func<Task> BuildHandler(IServiceProvider serviceProvider, Func<Task> next)
        {
            var channel = serviceProvider.GetService<ITextChannel>();
            
            return async () =>
            {
                await channel.SendLineAsync(options.Message);

                await next();
            };
        }
    }
}
