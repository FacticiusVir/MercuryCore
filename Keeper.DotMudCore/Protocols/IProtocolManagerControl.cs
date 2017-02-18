using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols
{
    public interface IProtocolManagerControl
        : IProtocolManager
    {
        Task MakeActiveAsync(IProtocol protocol);
    }

    public static class ProtocolManagerControlExtensions
    {
        public static Task MakeActiveAsync<T>(this IProtocolManagerControl protocolControl)
            where T : class, IProtocol
        {
            return protocolControl.MakeActiveAsync(protocolControl.Get<T>());
        }
    }
}
