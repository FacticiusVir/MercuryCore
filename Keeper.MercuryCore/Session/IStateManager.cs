namespace Keeper.MercuryCore.Session
{
    public interface IStateManager
    {
        T Get<T>();

        bool TryGet<T>(out T value);

        void Remove<T>();

        void Set<T>(T value);
    }
}
