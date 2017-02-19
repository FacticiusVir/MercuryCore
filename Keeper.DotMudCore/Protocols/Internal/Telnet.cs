using Keeper.DotMudCore.Dataflow;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.DotMudCore.Protocols.Internal
{
    internal class Telnet
        : ITelnet
    {
        private readonly ILogger<Telnet> logger;
        private readonly IConnection connection;
        private readonly IProtocolManagerControl protocolControl;

        private readonly IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;

        public Telnet(ILogger<Telnet> logger, IProtocolManagerControl protocolControl, IConnection connection)
        {
            this.logger = logger;
            this.connection = connection;
            this.protocolControl = protocolControl;

            this.lineAccumulator = LineAccumulatorBlock.Create(new Dictionary<byte, Func<Func<byte, bool>>>
            {
                { (byte)0xff, this.IacHandler}
            });
        }

        private Func<byte, bool> IacHandler()
        {
            var bytes = new List<byte>();

            return datum =>
            {
                bytes.Add(datum);

                if (bytes.Count == 2)
                {
                    this.logger.LogDebug($"IAC {(TelnetCommand)bytes[0]} {bytes[1]}");

                    return true;
                }

                return false;
            };
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
