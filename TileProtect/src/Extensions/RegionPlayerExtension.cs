using Amethyst.Commands;
using Amethyst.Core;
using Amethyst.Players;
using Amethyst.Players.Extensions;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Creative;
using TileProtect.Models;

namespace TileProtect.Extensions;

public sealed class RegionPlayerExtension : IPlayerExtension
{
    internal RegionPlayerExtension(NetPlayer plr)
    {
        Player = plr;
        oldRegionsEntered = [];
    }

    public NetPlayer Player { get; }

    internal DateTime? _notifyDelay;
    internal DateTime? _rgUpdateDelay;

    internal byte pointToDo;
    internal Point? point1;
    internal Point? point2;
    internal bool godModeWasEnabled;
    internal List<RegionModel> oldRegionsEntered;

    internal void UpdateRegions()
    {
        if (_rgUpdateDelay == null || _rgUpdateDelay < DateTime.UtcNow)
        {
            _rgUpdateDelay = DateTime.UtcNow.AddSeconds(1);
        }
        else
        {
            return;
        }

        int x = (int)(Player.TPlayer.position.X / 16);
        int y = (int)(Player.TPlayer.position.Y / 16);
        var point = new Point(x, y);

        List<RegionModel> cachedRegions = ProtectionModule._cachedRegions;
        List<RegionModel> regions = cachedRegions.FindAll(p => p.GetRectangle().Contains(point));

        foreach (RegionModel rg in oldRegionsEntered)
        {
            if (!regions.Any(p => rg.Name == p.Name))
            {
                rg.LeaveMessages.ForEach(HandleInteractionMessage);
                rg.LeaveCommands.ForEach(HandleInteractionCommand);
            }
        }

        foreach (RegionModel rg in regions)
        {
            if (!oldRegionsEntered.Any(p => rg.Name == p.Name))
            {
                rg.EnterMessages.ForEach(HandleInteractionMessage);
                rg.EnterCommands.ForEach(HandleInteractionCommand);
            }
        }

        oldRegionsEntered = regions;

        bool godMode = regions.Any(p => p.AutoGodMode);
        UpdateGodMode(godMode);
    }

    private void HandleInteractionMessage(string msg)
    {
        Player.ReplyMessage(msg, Color.White);
    }

    private void HandleInteractionCommand(RegionCommand command)
    {
        if (command.Permission != null && !Player.HasPermission(command.Permission))
        {
            return;
        }

        ICommandSender sender = command.CommandType == RegionCommandType.Self ? Player : CommandsManager.ConsoleSender;
        CommandsManager.RequestRun(sender, string.Format(command.Command, Player.Index));
    }

    internal void UpdateGodMode(bool godMode)
    {
        if (!godModeWasEnabled && !godMode)
        {
            return;
        }

        CreativePowers.GodmodePower power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();

        bool isEnabled = power.IsEnabledForPlayer(Player.Index);
        if (isEnabled != godMode)
        {
            power.SetEnabledState(Player.Index, isEnabled);
            godModeWasEnabled = godMode;
        }
    }

    public void Load() {}
    public void Unload() {}
}