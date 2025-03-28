using Amethyst.Network;
using Amethyst.Players;

namespace Amethyst.Talk.Rendering;

public sealed class RenderOperation
{
    internal RenderOperation(NetPlayer player, string text)
    {
        Player = player;

        Name = player.Name;
        Text = text;
        MessageFormat = "{0}{1}{2}: {3} {4}"; // {0} - prefix, {1} - name, {2} - suffix, {3} - text, {4} - postfix

        Prefix = new List<RenderSnippet>(8);
        Suffix = new List<RenderSnippet>(8);
        Postfix = new List<RenderSnippet>(8);

        Color = new NetColor(255, 255, 255);
    }

    public readonly NetPlayer Player;

    public string MessageFormat;

    public string Name;
    public string Text;

    public NetColor Color;

    public List<RenderSnippet> Prefix;
    public List<RenderSnippet> Suffix;
    public List<RenderSnippet> Postfix;
}