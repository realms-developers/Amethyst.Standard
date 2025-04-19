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