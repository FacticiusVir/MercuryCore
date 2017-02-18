using Nito.AsyncEx;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols.Internal
{
    internal class Telnet
        : ITelnet
    {
        private readonly IConnection connection;
        private readonly IProtocolManagerControl protocolControl;

        private readonly AsyncLock sendLock = new AsyncLock();
        private readonly AsyncLock receiveLock = new AsyncLock();

        private byte[] receiveBuffer = new byte[1024];
        private int receiveBufferCount = 0;
        private bool receiveLineWaiting = false;

        public Telnet(IProtocolManagerControl protocolControl, IConnection connection)
        {
            this.connection = connection;
            this.protocolControl = protocolControl;
        }

        public async Task<string> ReceiveLineAsync()
        {
            await MakeActiveAsync();

            using (await this.receiveLock.LockAsync())
            {
                int lineCount = 0;

                int newLineSize = 1;

                if (this.receiveLineWaiting)
                {
                    (lineCount, newLineSize) = ScanForNewLine(0, this.receiveBufferCount);
                }
                else
                {
                    while (!this.receiveLineWaiting)
                    {
                        int count = await this.connection.ReceiveAsync(this.receiveBuffer, this.receiveBufferCount);

                        if (count == 0)
                        {
                            throw new ClientDisconnectedException();
                        }

                        (lineCount, newLineSize) = ScanForNewLine(this.receiveBufferCount, this.receiveBufferCount + count);

                        this.receiveBufferCount += count;
                    }
                }

                string message = Encoding.ASCII.GetString(this.receiveBuffer, 0, lineCount);

                this.receiveLineWaiting = false;

                int offset = lineCount + newLineSize;

                for (int index = 0; index + offset < this.receiveBufferCount; index++)
                {
                    this.receiveBuffer[index] = this.receiveBuffer[index + offset];

                    if (this.receiveBuffer[index] == '\n')
                    {
                        this.receiveLineWaiting = true;
                    }
                }

                this.receiveBufferCount -= offset;

                return message;
            }
        }

        private (int lineCount, int newLineSize) ScanForNewLine(int offset, int count)
        {
            int lineCount = offset;
            int newLineSize = 1;

            while(lineCount < count && !this.receiveLineWaiting)
            {
                if (this.receiveBuffer[lineCount] == '\n')
                {
                    this.receiveLineWaiting = true;

                    if (this.receiveBuffer[lineCount - 1] == '\r')
                    {
                        lineCount--;
                        newLineSize = 2;
                    }
                }
                else
                {
                    lineCount++;
                }
            }

            return (lineCount, newLineSize);
        }

        public async Task SendAsync(string message)
        {
            await MakeActiveAsync();

            using (await this.sendLock.LockAsync())
            {
                await this.connection.SendAsync(Encoding.ASCII.GetBytes(message));
            }
        }

        public async Task MakeActiveAsync()
        {
            await this.protocolControl.MakeActiveAsync(this);
        }
    }
}
