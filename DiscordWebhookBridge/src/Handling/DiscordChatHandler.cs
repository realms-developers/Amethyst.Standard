using Amethyst.Kernel;
using Amethyst.Network;
using Amethyst.Network.Handling.Base;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Players;
using DSharpPlus.Entities;

namespace DiscordWebhookBridge.Handling;

internal class DiscordChatHandler : INetworkHandler
{
    public string Name => typeof(DiscordChatHandler).FullName!;

    public void Load() => NetworkManager.AddDirectHandler(82, OnReadNetModule);

    public void Unload() => NetworkManager.RemoveDirectHandler(82, OnReadNetModule);

    private static void OnReadNetModule(PlayerEntity plr, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        FastPacketReader reader = new(rawPacket, 3);
        ushort type = reader.ReadUInt16();

        if (type != 1)
        {
            return;
        }

        string cmd = reader.ReadString();

        if (cmd.Length > 64)
        {
            return;
        }

        string message = reader.ReadString();

        if (cmd != "Say" || message.StartsWith(AmethystSession.Profile.CommandPrefix))
        {
            return;
        }

        string content = DiscordWebhookBridge._webhookcfg.Data.AllowPinging ? message : message.Replace("@", "@\u200B");

        DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
            .WithUsername(plr.Name)
            .WithContent(content);

        DiscordWebhookBridge._client.BroadcastMessageAsync(builder).Wait();
    }
}
