using System.Reflection;
using System.Text;
using System.Timers;
using Amethyst;
using Amethyst.Core;
using Amethyst.Extensions.Modules;
using Amethyst.Extensions.Plugins;
using Amethyst.Players;
using Amethyst.Storages.Config;
using StatusBar.Configuration;
using Timer = System.Timers.Timer;

namespace StatusBar;

[AmethystModule(nameof(StatusBar))]
public static class StatusBar
{
    internal const string _methodName = "RenderStatusText";

    internal static readonly Configuration<StatusBarConfiguration> _sbCfg = new(typeof(StatusBarConfiguration).FullName!, new());

    internal static Func<NetPlayer, string>?[] delegates = null!;

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

        delegates = new Func<NetPlayer, string>?[_sbCfg.Data.Plugins.Length];

        PluginLoader.OnPluginLoad += OnPluginLoad;
        PluginLoader.OnPluginUnload += OnPluginUnload;

        _updateTimer = new Timer(_sbCfg.Data.UpdateIntervalMs)
        {
            AutoReset = true,
            Enabled = true
        };

        _updateTimer.Elapsed += Update;
    }

    private static void OnPluginLoad(PluginContainer container)
    {
        if (container.PluginInstance == null)
        {
            return;
        }

        // Find the index of this plugin in the configuration
        int pluginIndex = Array.IndexOf(_sbCfg.Data.Plugins, container.PluginInstance.Name);

        if (pluginIndex == -1)
        {
            AmethystLog.Main.Debug(nameof(OnPluginLoad), $"{container.PluginInstance.Name} not found in status bar configuration.");

            return;
        }

        try
        {
            // Get the type containing the Render method
            Type? pluginType = container.Assembly.GetTypes()
                .FirstOrDefault(t => t.GetMethod(_methodName,
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    [typeof(NetPlayer)],
                    null) != null);

            if (pluginType == null)
            {
                AmethystLog.Main.Debug(nameof(OnPluginLoad), $"No render type for plugin {container.PluginInstance.Name} found.");

                return;
            }

            // Get the static Render method
            MethodInfo? renderMethod = pluginType.GetMethod(
                _methodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                [typeof(NetPlayer)],
                null);

            if (renderMethod == null)
            {
                AmethystLog.Main.Debug(nameof(OnPluginLoad), $"No render method for plugin {container.PluginInstance.Name} found.");

                return;
            }

            // Create the delegate
            var renderDelegate = (Func<NetPlayer, string>)Delegate.CreateDelegate(
                typeof(Func<NetPlayer, string>),
                renderMethod);

            // Store the delegate in the array
            delegates[pluginIndex] = renderDelegate;
        }
        catch (Exception ex)
        {
            // Log error
            AmethystLog.Main.Error(nameof(OnPluginLoad), $"Failed to create delegate for plugin {container.PluginInstance.Name}: {ex.Message}");
        }
    }

    private static void OnPluginUnload(PluginContainer container)
    {
        if (container.PluginInstance == null)
        {
            return;
        }

        // Find the index of this plugin in the configuration
        int pluginIndex = Array.IndexOf(_sbCfg.Data.Plugins, container.PluginInstance.Name);

        if (pluginIndex == -1 || pluginIndex >= delegates.Length)
        {
            // Plugin not found in configuration or index out of bounds
            return;
        }

        try
        {
            // Clear the delegate reference
            delegates[pluginIndex] = null;

            AmethystLog.Main.Debug(nameof(OnPluginUnload),
                $"Cleared delegate for plugin: {container.PluginInstance.Name}");
        }
        catch (Exception ex)
        {
            AmethystLog.Main.Error(nameof(OnPluginUnload),
                $"Failed to unload plugin {container.PluginInstance.Name}: {ex.Message}");
        }
    }

    private static void Update(object? source, ElapsedEventArgs e)
    {
        foreach (NetPlayer plr in PlayerManager.Tracker)
        {
            string statusText = Render(plr);

            plr.Utils.SendStatusText(statusText, _sbCfg.Data.Padding);
        }
    }

    internal static string Render(NetPlayer from)
    {
        StatusBarConfiguration config = _sbCfg.Data;

        var builder = new StringBuilder();

        // Add header if not empty
        if (!string.IsNullOrEmpty(config.Header))
        {
            builder.Append(Localization.Get(config.Header, from.Language));
        }

        var results = new List<string>();

        foreach (Func<NetPlayer, string>? renderDelegate in delegates)
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
            builder.Append(Localization.Get(config.Footer, from.Language));
        }

        return builder.ToString();
    }
}
