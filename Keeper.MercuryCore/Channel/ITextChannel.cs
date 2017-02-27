using System.Threading.Tasks;

namespace Keeper.MercuryCore.Channel
{
    public interface ITextChannel
        : IChannel
    {
        Task SendAsync(string message);
    }
}

namespace Keeper.MercuryCore
{
    public static class TextChannelExtensions
    {
        public static Task SendLineAsync(this Channel.ITextChannel channel, string message)
        {
            return channel.SendAsync(message + "\r\n");
        }
    }
}
