using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class ServerBuilder
        : IServerBuilder
    {
        private List<Func<SessionDelegate, SessionDelegate>> middlewares = new List<Func<SessionDelegate, SessionDelegate>>();

        public IServiceProvider Services
        {
            get;
            set;
        }

        public IServerBuilder Use(Func<SessionDelegate, SessionDelegate> middleware)
        {
            this.middlewares.Add(middleware);

            return this;
        }

        public SessionDelegate Build()
        {
            SessionDelegate app = conn => Task.CompletedTask;

            foreach (var middleware in this.middlewares.AsEnumerable().Reverse())
            {
                app = middleware(app);
            }

            return app;
        }
    }
}
