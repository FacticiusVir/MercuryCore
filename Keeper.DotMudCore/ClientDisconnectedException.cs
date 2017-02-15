using System;

namespace Keeper.DotMudCore
{
    public class ClientDisconnectedException
        : Exception
    {
        public ClientDisconnectedException()
            : this("Client disconnected unexpectedly.")
        {
        }

        public ClientDisconnectedException(string message)
            : base(message)
        {
        }

        public ClientDisconnectedException(Exception innerException)
            : base("Client disconnected unexpectedly.", innerException)
        {
        }
    }
}
