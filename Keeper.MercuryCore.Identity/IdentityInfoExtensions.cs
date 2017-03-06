using Keeper.MercuryCore.Identity;
using Keeper.MercuryCore.Session;

namespace Keeper.MercuryCore
{
    public static class IdentityInfoExtensions
    {
        public static IdentityInfo GetIdentityInfo(this IStateManager sessionState)
        {
            return sessionState.Get<IdentityInfo>();
        }

        public static bool TryGetIdentityInfo(this IStateManager sessionState, out IdentityInfo info)
        {
            return sessionState.TryGet(out info);
        }
    }
}
