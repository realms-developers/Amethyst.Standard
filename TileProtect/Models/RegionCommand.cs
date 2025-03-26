namespace Amethyst.TileProtect.Models;

public record RegionCommand(RegionCommandType CommandType, string Command, string? Permission);