using Amethyst.Server.Entities.Players;
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
        string username = Render(message, _webhookcfg.Data.UserFormat);
        string output = Render(message, _webhookcfg.Data.ContentFormat);

        if (!_webhookcfg.Data.AllowPinging)
        {
            output = output.Replace("@", "@\u200B");
        }

        DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
            .WithUsername(username.RemoveColorTags())
            .WithContent(output.RemoveColorTags());

        string avatarUrl = GetAvatarUrl(message.Entity);

        if (!string.IsNullOrEmpty(avatarUrl))
        {
            builder.WithAvatarUrl(avatarUrl);
        }

        _client.BroadcastMessageAsync(builder).Wait();
    }

    private string GetAvatarUrl(PlayerEntity player)
    {
        if (player == null || player.User == null || player.User.Messages == null)
        {
            return string.Empty;
        }

        string? avatarUrl = null;

        foreach (var permission in _webhookcfg.Data.PermissionedAvatars)
        {
            if (player.User?.Permissions.HasPermission(permission.Key) == Amethyst.Systems.Users.Base.Permissions.PermissionAccess.HasPermission)
            {
                avatarUrl = permission.Value;
                break;
            }
        }

        return avatarUrl ?? _webhookcfg.Data.DefaultAvatarUrl;
    }

    private string Render(MessageRenderResult message, string format)
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

        List<string> configParts = ParseConfig(format);

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
                        format = format.Replace(part, value1);

                        canContinue = true;
                        break;
                    }
                }

                if (canContinue)
                    continue;
            }


            format = GetValueOrDefault(key, out var value2) ?
                format.Replace(part, value2) :
                format.Replace(part, string.Empty);
        }

        format = format.Trim('{').Trim('}');

        return format;
    }

    private List<string> ParseConfig(string format)
    {
        var parts = new List<string>();
        int startIndex = 0;

        while (startIndex < format.Length)
        {
            int openBraceIndex = format.IndexOf('{', startIndex);
            if (openBraceIndex == -1)
            {
                parts.Add(format.Substring(startIndex));
                break;
            }

            if (openBraceIndex > startIndex)
            {
                parts.Add(format.Substring(startIndex, openBraceIndex - startIndex));
            }

            int closeBraceIndex = format.IndexOf('}', openBraceIndex);
            if (closeBraceIndex == -1)
            {
                parts.Add(format.Substring(openBraceIndex));
                break;
            }

            parts.Add(format.Substring(openBraceIndex, closeBraceIndex - openBraceIndex + 1));
            startIndex = closeBraceIndex + 1;
        }

        return parts;
    }
}
