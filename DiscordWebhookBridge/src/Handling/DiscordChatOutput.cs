using Amethyst.Systems.Chat.Base;
using Amethyst.Systems.Chat.Base.Models;
using DSharpPlus.Entities;
using static DiscordWebhookBridge.DiscordWebhookBridge;

namespace DiscordWebhookBridge.Handling;

public sealed class DiscordChatOutput : IChatMessageOutput
{
    public string Name => typeof(DiscordChatOutput).FullName!;

    public void OutputMessage(MessageRenderResult message)
    {
        // Content selection
        string content = message.Text.GetValueOrDefault("modifiedtext")
                      ?? message.Text.GetValueOrDefault("realtext")
                      ?? string.Join(" ", message.Text.Values);

        // Apply anti-ping protection if needed
        if (!_webhookcfg.Data.AllowPinging)
        {
            content = content.Replace("@", "@\u200B");
        }

        // Format content
        content = string.Format(_webhookcfg.Data.ContentFormat, content,
                message.Entity.Name, message.Entity.UUID, message.Entity.IP, message.Entity.Index);

        // Build and send webhook
        DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
            .WithUsername(message.Entity.Name)
            .WithContent(content);

        if (_webhookcfg.Data.AvatarUrl != string.Empty)
        {
            builder.WithAvatarUrl(_webhookcfg.Data.AvatarUrl);
        }

        _client.BroadcastMessageAsync(builder).Wait();
    }
}
