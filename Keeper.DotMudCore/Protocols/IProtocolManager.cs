namespace Keeper.DotMudCore.Protocols
{
    public interface IProtocolManager
    {
        void MarkSupport<T, V>()
            where T : class, IProtocol
            where V: T;

        void MarkNotSupported<T>()
            where T : class, IProtocol;

        ProtocolSupport GetSupport<T>() where T : class, IProtocol;

        T Get<T>() where T : class, IProtocol;

        IProtocol Active { get; }
    }
}
