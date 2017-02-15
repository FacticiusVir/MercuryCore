namespace Keeper.DotMudCore
{
    public interface ISession
    {
        IConnection Connection { get; }

        ISessionStateManager State { get; }
    }
}
