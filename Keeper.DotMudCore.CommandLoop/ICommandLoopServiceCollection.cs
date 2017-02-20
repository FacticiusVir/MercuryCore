﻿namespace Keeper.DotMudCore.CommandLoop
{
    public interface ICommandLoopServiceCollection
    {
        ICommandLoopServiceCollection AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}