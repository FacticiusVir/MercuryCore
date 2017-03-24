using System.Security.Cryptography.X509Certificates;

namespace Keeper.MercuryCore.Tcp
{
    public class TcpOptions
    {
        public int Port
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
