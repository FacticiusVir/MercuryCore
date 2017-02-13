namespace Keeper.DotMudCore
{
    public static class IdentityInfoExtensions
    {
        public static IdentityInfo GetIdentityInfo(this ISession session)
        {
            return session.GetState<IdentityInfo>();
        }

        public static bool TryGetIdentityInfo(this ISession session, out IdentityInfo info)
        {
            return session.TryGetState(out info);
        }

        public static void RemoveIdentityInfo(this ISession session)
        {
            session.RemoveState<IdentityInfo>();
        }

        public static void SetIdentityInfo(this ISession session, IdentityInfo info)
        {
            session.SetState(info);
        }
    }
}
