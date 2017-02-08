using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface ILoginManager
    {
        Task<LoginResult> Login(IConnection connection);
    }
}
