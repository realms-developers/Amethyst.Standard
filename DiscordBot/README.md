# DiscordBot

Amethyst module for Discord bot integration using DSharpPlus.

## Configuration

### Client Configuration

| Property      | Type            | Default Value               | Description                          |
|---------------|-----------------|-----------------------------|--------------------------------------|
| `Token`       | string          | `string.Empty`              | Discord bot token                    |
| `TokenType`   | `TokenType`     | `TokenType.Bot`             | Type of token (usually Bot)          |
| `Intents`     | `DiscordIntents`| `DiscordIntents.AllUnprivileged` | Discord gateway intents        |
| `ShardId`     | int             | `0`                         | Shard ID for sharded bots            |
| `ShardCount`  | int             | `1`                         | Total number of shards               |

### Status Configuration

| Property        | Type            | Default Value       | Description                          |
|-----------------|-----------------|---------------------|--------------------------------------|
| `UserStatus`    | `UserStatus`    | `UserStatus.Online` | Initial bot status                   |
| `StatusText`    | string          | `string.Empty`      | Status text to display               |
| `ActivityType`  | `ActivityType`  | `ActivityType.Playing` | Type of activity to display      |

## Commands

| Command               | Description                                  | Permission |
|-----------------------|----------------------------------------------|------------|
| `discord-info`        | Displays information about the Discord bot   | None       |

## Example Usage

```csharp
using Amethyst.Extensions.Plugins;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using static DiscordBot.DiscordBot;

namespace DiscordTestBot;

public sealed class DiscordTestBot : PluginInstance
{
    public override string Name => "DiscordTestBot";
    public override Version Version => new(1, 0);

    protected override void Load()
    {
        CommandsNextExtension ext = Client.UseCommandsNext(new()
        {
            StringPrefixes = ["!"]
        });

        ext.RegisterCommands<DiscordCommandsModule>();
        Connect();
    }

    protected override void Unload() => Disconnect();
}

public class DiscordCommandsModule : BaseCommandModule
{
    [Command("greet")]
    public async Task GreetCommand(CommandContext ctx) => await ctx.RespondAsync("Greetings!");
}
```