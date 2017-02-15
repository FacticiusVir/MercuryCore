using Keeper.DotMudCore.Identity;

namespace Keeper.DotMudCore
{
    public static class IdentityInfoExtensions
    {
        public static IdentityInfo GetIdentityInfo(this ISession session)
        {
            return session.State.Get<IdentityInfo>();
        }

        public static bool TryGetIdentityInfo(this ISession session, out IdentityInfo info)
        {
            return session.State.TryGet(out info);
        }
    }
}
