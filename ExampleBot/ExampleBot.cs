﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using InteractivityAddon;
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

        public async Task Startasync()
        {
            string discordToken = "";

            await Client.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            await Client.StartAsync();                                // Start message receiving

            Client.Log += x => {
                Console.WriteLine(x.Message);
                return Task.CompletedTask;
            };
            Commands.CommandErrored += (result, ctx, provider) => {

                Console.WriteLine(result.Exception.ToString());

                return Task.CompletedTask;
            };

            Client.MessageReceived += async s => {
                if (!(s is SocketUserMessage msg)) {
                    return; //Do some checks
                }

                if (msg.Author.IsBot) {
                    return;
                }

                if (msg.Author.Id == Client.CurrentUser.Id) {
                    return;
                }

                var Context = new ExampleCommandContext(msg);

                if (!CommandUtilities.HasAnyPrefix(msg.Content, new[] { "!" }, StringComparison.OrdinalIgnoreCase, out string UsedPrefix, out string cmd) == true) {
                    return;
                }

                var Result = await Commands.ExecuteAsync(cmd, Context, Provider); //Try to run Command

                if (Result is FailedResult FailResult) {
                    await Context.Channel.SendMessageAsync(FailResult.Reason);
                }

                return;
            };

            await Task.Delay(-1);                                     //Wait forever to keep the bot running
        }

        private IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Verbose, MessageCacheSize = 50 });

            return services
                .AddSingleton(Client)
                .AddSingleton(new CommandService(new CommandServiceConfiguration { CaseSensitive = false, IgnoreExtraArguments = false, DefaultRunMode = RunMode.Parallel }))
                .AddSingleton(new InteractivityService(Client, TimeSpan.FromSeconds(20)))
                .BuildServiceProvider();
        }
    }
}