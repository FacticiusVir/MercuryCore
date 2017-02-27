using System;
using System.Threading.Tasks;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Internal;
using System.Threading.Tasks.Dataflow;
using System.Text;

namespace Keeper.MercuryCore.Channel
{
    public class AsciiChannel
        : ITextChannel, IChannel
    {
        private IConnection connection;

        public IDisposable Bind(IConnection connection)
        {
            this.connection = connection;

            return new DelegateDisposable(() => { });
        }

        public Task SendAsync(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);

            return this.connection.Send.SendAsync(new ArraySegment<byte>(data));
        }
    }
}
