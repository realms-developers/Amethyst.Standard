using Amethyst.Extensions.Plugins;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Server.Entities.Players;
using Amethyst;
using Amethyst.Kernel;
using Amethyst.Hooks;
using Amethyst.Hooks.Args.Utility;
using Amethyst.Hooks.Base;

namespace WorldRegeneration;

[ExtensionMetadata("WorldRegeneration", "realms-developers", "Provides world regeneration for Amethyst.API Terraria servers")]
public sealed class PluginMain : PluginInstance
{
    internal static DateTime NextRegenerationTime { get; set; } = DateTime.UtcNow.AddMinutes(RegenConfiguration.Instance.AutoRegenerateMinutes);

    protected override void Load()
    {
        HookRegistry.GetHook<SecondTickArgs>()
            .Register(OnSecondTick);
    }

    private void OnSecondTick(in SecondTickArgs args, HookResult<SecondTickArgs> result)
    {
        if (RegenConfiguration.Instance.AutoRegenerate && DateTime.UtcNow >= NextRegenerationTime)
        {
            if (RegenUtils.LoadWorld(RegenUtils.GetDefaultWorldPath()))
            {
                NextRegenerationTime = DateTime.UtcNow.AddMinutes(RegenConfiguration.Instance.AutoRegenerateMinutes);
                AmethystLog.Main.Info("WorldRegeneration", "World regeneration completed successfully.");
            }
            else
            {
                AmethystLog.Main.Error("WorldRegeneration", "Failed to load the world for regeneration.");
            }
        }
    }

    protected override void Unload()
    {
        HookRegistry.GetHook<SecondTickArgs>()
            .Unregister(OnSecondTick);
    }

    public static string RenderStatusText(PlayerEntity from)
    {
        if (NextRegenerationTime <= DateTime.UtcNow)
            return Localization.Get("wregen.status.ready", from.User?.Messages.Language ?? AmethystSession.Profile.DefaultLanguage);

        var remainingTime = NextRegenerationTime - DateTime.UtcNow;
        return string.Format(Localization.Get("wregen.status", from.User?.Messages.Language ?? AmethystSession.Profile.DefaultLanguage), (int)remainingTime.TotalHours, remainingTime.Minutes);
    }
}
