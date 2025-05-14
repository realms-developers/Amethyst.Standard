using Amethyst.Commands;
using Amethyst.Core;
using Amethyst.Extensions.Plugins;
using Amethyst.Players;
using Microsoft.Extensions.Configuration;

namespace Handbook;

public sealed class Handbook : PluginInstance
{
    private static readonly string _handbookPath = Path.Combine(AmethystSession.Profile.SavePath, nameof(Handbook));

    public override string Name => nameof(Handbook);

    public override Version Version => new(1, 0);

    protected override void Load()
    {
        try
        {
            if (!Directory.Exists(_handbookPath))
            {
                Directory.CreateDirectory(_handbookPath);

                string samplePath = Path.Combine(_handbookPath, "handbook.ini");

                const string sampleContent = """
                ; Example handbook.ini
                [meta]
                description = Displays a welcome message ; Command description. Supports localization
                permission = handbook.admin              ; Optional permission requirement

                [en-US]                                  ; Language code
                content = Welcome!                       ; Displayed content
                
                [ru-RU]
                content = приветствовать!
                """;

                File.WriteAllText(samplePath, sampleContent);

                AmethystLog.Main.Info(Name, $"Created sample handbook at: {samplePath}");
            }

            string[] iniFiles = Directory.GetFiles(_handbookPath, "*.ini");

            if (iniFiles.Length == 0)
            {
                AmethystLog.Main.Warning(Name, "No handbook files found in directory");

                return;
            }

            foreach (string file in iniFiles)
            {
                string commandName = Path.GetFileNameWithoutExtension(file);

                IConfigurationRoot config = new ConfigurationBuilder()
                .AddIniFile(file)
                .Build();

                string description = config["meta:description"] ?? string.Empty;
                string? permission = config["meta:permission"];

                bool registered = RegisterCommand(new(null, commandName, description,
                    typeof(Handbook).GetMethod(nameof(ReadHandbook))!, 0, CommandType.Shared, permission, null));

                if (registered)
                {
                    AmethystLog.Main.Debug(Name, $"Registered handbook command: {commandName}");
                }
                else
                {
                    AmethystLog.Main.Error(Name, $"Handbook command {commandName} is registered already.");
                }
            }
        }
        catch (Exception ex)
        {
            AmethystLog.Main.Error(Name, $"Failed to load handbook: {ex.Message}");
        }
    }

    protected override void Unload() { }

    public static void ReadHandbook(CommandInvokeContext ctx)
    {
        string filePath = Path.Combine(_handbookPath, $"{ctx.Name}.ini");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Handbook file not found: {filePath}");
        }

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(_handbookPath)
            .AddIniFile(Path.GetFileName(filePath))
            .Build();

        string content = config[$"{ctx.Sender.Language}:content"]
                         ?? config[$"{AmethystSession.Profile.DefaultLanguage}:content"]
                         ?? "Content unavailable.";

        if (ctx.Sender is NetPlayer player)
        {
            player.SendMessage(content);
        }
        else
        {
            ctx.Sender.ReplyInfo(content);
        }
    }

    // TO DO: Handbook creation and manipulation via commands
}
