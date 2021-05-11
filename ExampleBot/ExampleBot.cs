using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace ExampleBot_Qmmands
{
    public class ExampleBot
    {
        public IServiceProvider Provider { get; private set; }
        public DiscordSocketClient Client { get; private set; }
        public CommandService Commands { get; private set; }

        public void Initialize()
        {
            IServiceCollection services = new ServiceCollection();
            Provider = ConfigureServices(services);

            Commands = Provider.GetRequiredService<CommandService>();
            Commands.AddModules(Assembly.GetEntryAssembly()); //Add Modules to the CommandService

            Client = Provider.GetRequiredService<DiscordSocketClient>();
        }

        public async Task StartAsync()
        {
            var discordToken = "Your Token Here";

            await Client.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            await Client.StartAsync();                                // Start message receiving

            Client.Log += x =>
            {
                Console.WriteLine(x.Message);
                return Task.CompletedTask;
            };
            Commands.CommandExecutionFailed += args =>
            {
                Console.WriteLine(args.Result.Exception.ToString());
                return Task.CompletedTask;
            };

            Client.MessageReceived += async s =>
            {
                if (s is not SocketUserMessage msg)
                    return; //Do some checks
                
                if (msg.Author.IsBot)
                    return;

                if (msg.Author.Id == Client.CurrentUser.Id)
                    return;

                var context = new ExampleCommandContext(msg, Provider);

                if (!CommandUtilities.HasAnyPrefix(msg.Content, new[] { "!" }, StringComparison.OrdinalIgnoreCase, out var usedPrefix, out var cmd))
                    return;
                
                var result = await Commands.ExecuteAsync(cmd, context); //Try to run Command

                if (result is FailedResult failResult)
                    await context.Channel.SendMessageAsync(failResult.FailureReason);
            };

            await Task.Delay(-1);                                     //Wait forever to keep the bot running
        }

        private IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Verbose, MessageCacheSize = 50 }))
                .AddSingleton(new CommandService(new CommandServiceConfiguration { DefaultRunMode = RunMode.Parallel }))
                .AddSingleton<InteractivityService>()
                .AddSingleton(new InteractivityConfig { DefaultTimeout = TimeSpan.FromSeconds(20), RunOnGateway = false})
                .BuildServiceProvider();
        }
    }
}