using System.Net;
using System.Security.Cryptography.X509Certificates;

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

        public X509FindType SslCertFind
        {
            get;
            set;
        }

        public string SslCertValue
        {
            get;
            set;
        }
    }
}