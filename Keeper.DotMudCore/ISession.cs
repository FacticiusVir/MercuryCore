namespace Keeper.DotMudCore
{
    public interface ISession
    {
        IConnection Connection { get; }

        T GetState<T>();

        bool TryGetState<T>(out T value);

        void RemoveState<T>();

        void SetState<T>(T value);
    }
}
