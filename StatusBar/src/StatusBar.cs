using System.Reflection;
using System.Text;
using System.Timers;
using Amethyst;
using Amethyst.Extensions.Base.Metadata;
using Amethyst.Extensions.Base.Result;
using Amethyst.Extensions.Modules;
using Amethyst.Hooks;
using Amethyst.Hooks.Args.Extensions;
using Amethyst.Hooks.Base;
using Amethyst.Kernel;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Storages.Config;
using StatusBar.Configuration;
using Timer = System.Timers.Timer;

namespace StatusBar;

[ExtensionMetadata(nameof(StatusBar), "realms-developers")]
public static class StatusBar
{
    private const string _methodName = "RenderStatusText";

    private static readonly Configuration<StatusBarConfiguration> _sbCfg = new(nameof(StatusBarConfiguration), new());

    private static Func<PlayerEntity, string>?[] delegates = null!;

    private static Timer? _updateTimer;

    private static bool _isInitialized;

    [ModuleInitialize]
    public static void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        _sbCfg.Load();

        delegates = new Func<PlayerEntity, string>?[_sbCfg.Data.Plugins.Length];

        HookRegistry.GetHook<PluginInitializeArgs>().Register(OnPluginInitialize);
        HookRegistry.GetHook<PluginDeinitializeArgs>().Register(OnPluginDeinitialize);

        _updateTimer = new Timer(_sbCfg.Data.UpdateIntervalMs)
        {
            AutoReset = true,
            Enabled = true
        };

        _updateTimer.Elapsed += Update;
    }

    private static void OnPluginInitialize(in PluginInitializeArgs args, HookResult<PluginInitializeArgs> result)
    {
        if (args.Result.State != ExtensionResult.SuccessOperation)
        {
            return;
        }

        string pluginName = args.Instance.Root.Metadata.Name;

        // Find the index of this plugin in the configuration
        int pluginIndex = Array.IndexOf(_sbCfg.Data.Plugins, pluginName);

        if (pluginIndex == -1)
        {
            AmethystLog.Main.Debug(nameof(OnPluginInitialize), $"{pluginName} not found in status bar configuration.");

            return;
        }

        try
        {
            // Get the type containing the Render method
            Type? pluginType = args.Instance.Root.Assembly.GetTypes()
                .FirstOrDefault(t => t.GetMethod(_methodName,
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    [typeof(PlayerEntity)],
                    null) != null);

            if (pluginType == null)
            {
                AmethystLog.Main.Debug(nameof(OnPluginInitialize), $"No render type for plugin {pluginName} found.");

                return;
            }

            // Get the static Render method
            MethodInfo? renderMethod = pluginType.GetMethod(
                _methodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                [typeof(PlayerEntity)],
                null);

            if (renderMethod == null)
            {
                AmethystLog.Main.Debug(nameof(OnPluginInitialize), $"No render method for plugin {pluginName} found.");

                return;
            }

            // Create the delegate
            var renderDelegate = (Func<PlayerEntity, string>)Delegate.CreateDelegate(
                typeof(Func<PlayerEntity, string>),
                renderMethod);

            // Store the delegate in the array
            delegates[pluginIndex] = renderDelegate;
        }
        catch (Exception ex)
        {
            // Log error
            AmethystLog.Main.Error(nameof(OnPluginInitialize), $"Failed to create delegate for plugin {pluginName}: {ex.Message}");
        }
    }

    private static void OnPluginDeinitialize(in PluginDeinitializeArgs args, HookResult<PluginDeinitializeArgs> result)
    {
        if (args.Result.State != ExtensionResult.SuccessOperation)
        {
            return;
        }

        string pluginName = args.Instance.Root.Metadata.Name;

        // Find the index of this plugin in the configuration
        int pluginIndex = Array.IndexOf(_sbCfg.Data.Plugins, pluginName);

        if (pluginIndex == -1 || pluginIndex >= delegates.Length)
        {
            // Plugin not found in configuration or index out of bounds
            return;
        }

        try
        {
            // Clear the delegate reference
            delegates[pluginIndex] = null;

            AmethystLog.Main.Debug(nameof(OnPluginDeinitialize),
                $"Cleared delegate for plugin: {pluginName}");
        }
        catch (Exception ex)
        {
            AmethystLog.Main.Error(nameof(OnPluginDeinitialize),
                $"Failed to unload plugin {pluginName}: {ex.Message}");
        }
    }

    private static void Update(object? source, ElapsedEventArgs e)
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            string statusText = Render(plr);

            plr.SendStatusText(statusText, _sbCfg.Data.Padding);
        }
    }

    internal static string Render(PlayerEntity from)
    {
        StatusBarConfiguration config = _sbCfg.Data;

        var builder = new StringBuilder();

        string lang = from.User?.Messages.Language ?? AmethystSession.Profile.DefaultLanguage;

        // Add header if not empty
        if (!string.IsNullOrEmpty(config.Header))
        {
            builder.Append(Localization.Get(config.Header, lang));
        }

        var results = new List<string>();

        foreach (Func<PlayerEntity, string>? renderDelegate in delegates)
        {
            if (renderDelegate != null)
            {
                try
                {
                    string result = renderDelegate(from);
                    if (!string.IsNullOrEmpty(result))
                    {
                        results.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    AmethystLog.Main.Error(nameof(Render),
                        $"Error executing plugin delegate: {ex.Message}");
                }
            }
        }

        // Add results with separator
        if (results.Count > 0)
        {
            builder.Append(string.Join(config.Separator, results));
        }

        // Add footer if not empty
        if (!string.IsNullOrEmpty(config.Footer))
        {
            builder.Append(Localization.Get(config.Footer, lang));
        }

        return builder.ToString();
    }
}
