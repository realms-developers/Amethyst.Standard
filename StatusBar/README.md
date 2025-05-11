# StatusBar

Amethyst module for displaying customizable status information in the player's status bar, with support for plugin integration.

## Configuration

| Property           | Type            | Default Value | Description                                                                 |
|--------------------|-----------------|---------------|-----------------------------------------------------------------------------|
| `Header`          | string          | `string.Empty` | Text to display at the beginning of the status bar (supports localization) |
| `Footer`          | string          | `string.Empty` | Text to display at the end of the status bar (supports localization)       |
| `Separator`       | string          | `"\n"`        | Separator used between different plugin outputs                             |
| `Plugins`         | string[]        | `[]`          | Array of plugin names that provide status information                       |
| `UpdateIntervalMs`| long            | `500`         | Interval in milliseconds between status bar updates                         |
| `Padding`         | bool            | `false`       | Whether to add padding to the status text                               |

## Plugin Integration

Plugins can contribute to the status bar by implementing a static `RenderStatusText` method. Here's an example plugin:

```csharp
using Amethyst.Extensions.Plugins;
using Amethyst.Players;

namespace DisplayPos;

public sealed class DisplayPos : PluginInstance 
{
    public override string Name => nameof(DisplayPos);
    public override Version Version => new Version(1, 0);

    protected override void Load() { }
    protected override void Unload() { }

    public static string RenderStatusText(NetPlayer from) => $"Position: X={from.Utils.PosX}, Y={from.Utils.PosY}";
}
```