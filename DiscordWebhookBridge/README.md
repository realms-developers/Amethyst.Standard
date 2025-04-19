# DiscordWebhookBridge

Amethyst plugin for Discord webhook integration using DSharpPlus. This plugin bridges in-game chat messages to Discord via webhooks.

## Configuration

### Webhook Configuration

| Property       | Type      | Default Value | Description                                         |
|----------------|-----------|---------------|-----------------------------------------------------|
| `WebhookUri`   | string[]  | `[]`          | Array of Discord webhook URLs                       |
| `AllowPinging` | bool      | `false`       | Determines whether or not to escape `@` on messages |