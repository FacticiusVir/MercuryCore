using Keeper.DotMudCore.Dataflow;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.DotMudCore.Protocols.Internal
{
    internal class PlainAscii
        : IPlainAscii
    {
        private readonly IConnection connection;
        private readonly IProtocolManagerControl protocolControl;

        private readonly IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator = LineAccumulatorBlock.Create();

        public PlainAscii(IProtocolManagerControl protocolControl, IConnection connection)
        {
            this.connection = connection;
            this.protocolControl = protocolControl;
        }

        public async Task<string> ReceiveLineAsync()
        {
            await MakeActiveAsync();

            return await this.lineAccumulator.ReceiveAsync();
        }

        public async Task SendAsync(string message)
        {
            await MakeActiveAsync();

            await this.connection.Send.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)));
        }

        public async Task MakeActiveAsync()
        {
            await this.protocolControl.MakeActiveAsync(this);
        }
        
        IDisposable IProtocol.CreateActiveSession()
        {
            return this.connection.Receive.LinkTo(this.lineAccumulator, new DataflowLinkOptions { PropagateCompletion = true });
        }
    }
}
