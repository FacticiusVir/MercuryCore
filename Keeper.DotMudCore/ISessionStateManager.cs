namespace Keeper.DotMudCore
{
    public interface ISessionStateManager
    {
        T Get<T>();

        bool TryGet<T>(out T value);

        void Remove<T>();

        void Set<T>(T value);
    }
}
