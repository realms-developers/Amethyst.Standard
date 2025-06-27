using Amethyst.Extensions.Plugins;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Server.Entities.Players;
using Amethyst;
using Amethyst.Kernel;

namespace WorldRegeneration;

[ExtensionMetadata("WorldRegeneration", "realms-developers", "Provides world regeneration for Amethyst.API Terraria servers")]
public sealed class PluginMain : PluginInstance
{
    internal static DateTime NextRegenerationTime { get; private set; } = DateTime.UtcNow.AddMinutes(RegenConfiguration.Instance.AutoRegenerateMinutes);

    protected override void Load()
    {
    }

    protected override void Unload()
    {
    }

    public static string RenderStatusText(PlayerEntity from)
    {
        var remainingTime = NextRegenerationTime - DateTime.UtcNow;
        return string.Format(Localization.Get("wregen.status", from.User?.Messages.Language ?? AmethystSession.Profile.DefaultLanguage), (int)remainingTime.TotalHours, remainingTime.Minutes);
    }
}
