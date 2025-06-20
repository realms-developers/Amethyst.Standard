using Amethyst.Systems.Chat.Base;
using Amethyst.Systems.Chat.Base.Models;
using DSharpPlus.Entities;

namespace DiscordWebhookBridge.Handling;

public sealed class DiscordChatOutput : IChatMessageOutput
{
    public string Name => "discord.chatoutput";

    public void OutputMessage(MessageRenderResult message)
    {
        string msgText = string.Join(' ', message.Text);
        string content = DiscordWebhookBridge._webhookcfg.Data.AllowPinging ? msgText : msgText.Replace("@", "@\u200B");

        DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
            .WithUsername(message.Entity.Name)
            .WithContent(content);

        DiscordWebhookBridge._client.BroadcastMessageAsync(builder).Wait();
    }
}
