# Discord.InteractivityAddon

[![NuGet](https://img.shields.io/nuget/vpre/Discord.InteractivityAddon.svg?style=plastic)](https://www.nuget.org/packages/Discord.InteractivityAddon)
[![TechFacility](https://img.shields.io/discord/512366986383065088.svg?style=flat-square&label=discord)](https://discord.gg/H7FzMt3)

This is an addon for the Discord API Wrapper [Discord.Net](https://github.com/discord-net/Discord.Net) which makes it easy to add interactivity to your discord bot.

## Installation
The package is available to download on [NuGet](https://www.nuget.org/packages/Discord.InteractivityAddon).
Make sure to use the preview version of this package if you are planning to use the preview of Discord.Net

## Features
 - Waiting for a message / reaction which passes your filter
   - Run actions on filtered messages / reactions
 - Delayed sending and deleting messages & files
 - A powerful fully customizable Paginator
   - Send multi page messages
   - Move through pages using reactions
   - You can choose between a static and a lazy loaded one
   - Get more customizability by creating your own child of the `Paginator` & `PaginatorBuilder` classes
 - Fully customizable selection from a list of objects
   - Works with messages or reactions
   - Makes user input easy
   - Get more customizability by creating your own child of the `BaseMessageSelection` / `BaseReactionSelection` classes
 - Confirmation
 - Uptime counter
 
## Usage
To properly use the features this addon provides you need to add the `InteractivityService` to your service provider.

```cs
var provider = new ServiceCollection()
                .AddSingleton(new InteractivityService(Client, TimeSpan.FromMinutes(3)))
                ....
```
This addon does not include a custom `ModuleBase` in order to support every command framework (Discord.Net.Command/Qmmands/...). 

Inject the InteractivityService into your Module using DI instead. (Constructor / Public Property Injection)

### Example: Waiting for Messages / Reactions
```cs
[Command("nextmessage")]
public async Task ExampleReplyNextMessageAsync()
{
    var result = await Interactivity.NextMessageAsync(x => x.Author.Id == Context.User.Id);

    if (result.IsSuccess) {
        Interactivity.DelayedSendMessageAndDeleteAsync(
                        Context.Channel,
                        deleteDelay: TimeSpan.FromSeconds(20), 
                        text: result.Value.Content, 
                        embed: result.Value.Embeds.FirstOrDefault());
    }
}
```

### Example: Selection
```cs
[Command("select")]
public async Task ExampleReactionSelectionAsync()
{
    var builder = new ReactionSelectionBuilder<string>()
        .WithSelectables(new Dictionary<IEmote, string>()
        {
            [new Emoji("💵")] = "Hi",
            [new Emoji("🍭")] = "How",
            [new Emoji("😩")] = "Hey",
            [new Emoji("💠")] = "Huh?!"
        })
        .WithUsers(Context.User)
        .WithDeletion(DeletionOptions.AfterCapturedContext | DeletionOptions.Invalids);

    var result = await Interactivity.SendSelectionAsync(builder.Build(), Context.Channel, TimeSpan.FromSeconds(50));

    if (result.IsSuccess)
    {
        await Context.Channel.SendMessageAsync(result.Value.ToString());
    }
}
```

### Example: Pagination
```cs
[Command("paginator")]
public Task PaginatorAsync()
{
    var pages = new PageBuilder[] {
        new PageBuilder().WithTitle("I"),
        new PageBuilder().WithTitle("am"),
        new PageBuilder().WithTitle("cool"),
        new PageBuilder().WithTitle(":sunglasses:"),
        new PageBuilder().WithText("I am cool :crown:")
    };

    var paginator = new StaticPaginatorBuilder()
        .WithUsers(Context.User)
        .WithPages(pages)
        .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
        .WithDefaultEmotes()
        .Build();

    return Interactivity.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(2));
}
```

### Example: Confirmation
```cs
[Command("confirm")]
public async Task ConfirmAsync()
{
    var request = new ConfirmationBuilder()
        .WithContent(new PageBuilder().WithText("Please Confirm"))
        .Build();

    var result = await Interactivity.SendConfirmationAsync(request, Context.Channel);

    if (result.Value)
    {
        await Context.Channel.SendMessageAsync("Confirmed :thumbsup:!");
    }
    else
    {
        await Context.Channel.SendMessageAsync("Declined :thumbsup:!");
    }
}
```

## Troubleshooting

 >  A MessageReceived handler is blocking the gateway task.

If you are getting this error and you are using the `Discord.Commands` `ModuleBase` class as a base for your commands, make sure your commands are running asynchronously (eg. by using `RunMode = RunMode.Async`).