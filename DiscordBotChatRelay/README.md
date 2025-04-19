# DiscordBotChatRelay

An Amethyst plugin that relays messages from specified Discord channels to in-game chat using a Discord bot.

## Configuration

### Relay Configuration

| Property       | Type      | Default Value        | Description                                                                 |
|----------------|-----------|----------------------|-----------------------------------------------------------------------------|
| `ChannelIds`   | ulong[]   | `[]`                 | Array of Discord channel IDs whose messages should be relayed to the game.  |
| `Format`       | string    | `Discord> <{1}> {0}` | Format string for relayed messages. Available placeholders:                 |
|                |           |                      | - `{0}`: Message content                                                    |
|                |           |                      | - `{1}`: Author's username                                                  |
|                |           |                      | - `{2}`: Author's Discord ID                                                |
|                |           |                      | - `{3}`: Channel name                                                       |
|                |           |                      | - `{4}`: Channel ID                                                         |