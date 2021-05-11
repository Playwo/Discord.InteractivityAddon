using System;
using Discord.WebSocket;
using Qmmands;

namespace ExampleBot_Qmmands
{
    public class ExampleCommandContext : CommandContext
    {
        public ISocketMessageChannel Channel { get; }
        public SocketUser User { get; }
        public SocketGuild Guild { get; }

        public ExampleCommandContext(ISocketMessageChannel channel, SocketUser user, SocketGuild guild, IServiceProvider serviceProvider)
            : base (serviceProvider)
        {
            Channel = channel;
            User = user;
            Guild = guild;
        }
        public ExampleCommandContext(SocketMessage message, IServiceProvider serviceProvider)
            : base (serviceProvider)
        {
            Channel = message.Channel;
            User = message.Author;
            Guild = (User as SocketGuildUser)?.Guild;
        }
    }
}
