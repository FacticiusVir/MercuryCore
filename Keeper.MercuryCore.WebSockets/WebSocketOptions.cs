using System.Net;

namespace Keeper.MercuryCore.WebSockets
{
    public class WebSocketOptions
    {
        public IPAddress Address
        {
            get;
            set;
        } = IPAddress.Any;

        public int? Port
        {
            get;
            set;
        }
    }
}