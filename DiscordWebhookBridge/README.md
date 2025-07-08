# DiscordWebhookBridge

Amethyst plugin for Discord webhook integration using DSharpPlus. This plugin bridges in-game chat messages to Discord via webhooks.

## Configuration

### Webhook Configuration

| Property              | Type                    | Default Value                                                                 | Description                                                                 |
|-----------------------|-------------------------|-------------------------------------------------------------------------------|-----------------------------------------------------------------------------|
| `ContentFormat`       | string                 | `"{anvil.chat.customprefix|anvil.perms.prefix} {realname} {anvil.perms.suffix}: {modifiedtext|realtext}"` | Format string for the content of the message sent to Discord.              |
| `UserFormat`          | string                 | `"{anvil.chat.customprefix|anvil.perms.prefix} {realname}"`                  | Format string for the username displayed in Discord.                       |
| `PermissionedAvatars` | Dictionary<string, string> | `{}`                                                                        | Dictionary mapping permissions to specific avatar URLs.                    |
| `DefaultAvatarUrl`    | string                 | `""`                                                                        | Default avatar URL to use if no permissioned avatar is specified.          |
| `AllowPinging`        | bool                   | `false`                                                                     | Determines whether or not to escape `@` on messages.                       |