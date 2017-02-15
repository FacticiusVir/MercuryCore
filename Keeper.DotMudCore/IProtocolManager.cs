namespace Keeper.DotMudCore
{
    public interface IProtocolManager
    {
        void MarkSupport<T>(bool isSupported) where T : class;

        ProtocolSupport GetSupport<T>() where T : class;

        T Get<T>() where T : class;
    }
}
