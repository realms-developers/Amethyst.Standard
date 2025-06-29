using Amethyst.Systems.Chat.Base;
using Amethyst.Systems.Chat.Base.Models;
using Amethyst.Text;
using DSharpPlus.Entities;
using static DiscordWebhookBridge.DiscordWebhookBridge;

namespace DiscordWebhookBridge.Handling;

public sealed class DiscordChatOutput : IChatMessageOutput
{
    public string Name => typeof(DiscordChatOutput).FullName!;

    public void OutputMessage(MessageRenderResult message)
    {
        bool GetValueOrDefault(string key, out string? outValue)
        {
            if (message.Prefix.TryGetValue(key, out var value) ||
                message.Name.TryGetValue(key, out value) ||
                message.Suffix.TryGetValue(key, out value) ||
                message.Text.TryGetValue(key, out value))
            {
                outValue = value;
                return true;
            }

            outValue = null;
            return false;
        }

        List<string> configParts = ParseConfig();
        string output = DiscordWebhookBridge._webhookcfg.Data.ContentFormat;

        foreach (var part in configParts)
        {
            if (!part.StartsWith("{") || !part.EndsWith("}"))
            {
                continue;
            }

            bool canContinue = false;
            var key = part.Trim('{', '}');
            if (key.Contains('|'))
            {
                foreach (var subpart in key.Split('|'))
                {
                    if (GetValueOrDefault(subpart, out var value1))
                    {
                        output = output.Replace(part, value1);

                        canContinue = true;
                        break;
                    }
                }

                if (canContinue)
                    continue;
            }


            output = GetValueOrDefault(key, out var value2) ?
                output.Replace(part, value2) :
                output.Replace(part, string.Empty);
        }

        output = output.Trim('{').Trim('}');

        if (!_webhookcfg.Data.AllowPinging)
        {
            output = output.Replace("@", "@\u200B");
        }

        DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
            .WithUsername(message.Entity.Name)
            .WithContent(output.RemoveColorTags());

        if (_webhookcfg.Data.AvatarUrl != string.Empty)
        {
            builder.WithAvatarUrl(_webhookcfg.Data.AvatarUrl);
        }

        _client.BroadcastMessageAsync(builder).Wait();
    }


    private List<string> ParseConfig()
    {
        var config = DiscordWebhookBridge._webhookcfg.Data.ContentFormat;
        var parts = new List<string>();
        int startIndex = 0;

        while (startIndex < config.Length)
        {
            int openBraceIndex = config.IndexOf('{', startIndex);
            if (openBraceIndex == -1)
            {
                parts.Add(config.Substring(startIndex));
                break;
            }

            if (openBraceIndex > startIndex)
            {
                parts.Add(config.Substring(startIndex, openBraceIndex - startIndex));
            }

            int closeBraceIndex = config.IndexOf('}', openBraceIndex);
            if (closeBraceIndex == -1)
            {
                parts.Add(config.Substring(openBraceIndex));
                break;
            }

            parts.Add(config.Substring(openBraceIndex, closeBraceIndex - openBraceIndex + 1));
            startIndex = closeBraceIndex + 1;
        }

        return parts;   
    }
}
