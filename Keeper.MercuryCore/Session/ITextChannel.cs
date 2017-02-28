using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session
{
    public interface ITextChannel
        : IChannel
    {
        Task SendAsync(string message);

        Task<string> ReceiveLineAsync();
    }
}

namespace Keeper.MercuryCore
{
    public static class TextChannelExtensions
    {
        public static Task SendLineAsync(this Session.ITextChannel channel, string message)
        {
            return channel.SendAsync(message + "\r\n");
        }
    }
}
