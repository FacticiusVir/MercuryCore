namespace Keeper.DotMudCore.Protocols
{
    public interface IProtocolManager
    {
        void MarkSupport<T>(bool isSupported = true) where T : class, IProtocol;

        ProtocolSupport GetSupport<T>() where T : class, IProtocol;

        T Get<T>() where T : class, IProtocol;

        IProtocol Active { get; }
    }
}
