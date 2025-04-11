using Amethyst.Players;
using Microsoft.Xna.Framework;
using Talk.Rendering;
using Terraria;

namespace Talk;

public sealed class BasicChatProvider : IChatProvider
{
    public void HandlePlayerMessage(RenderResult renderResult) => PlayerUtilities.BroadcastText(renderResult.Message, renderResult.Color.ToXNA());

    public void HandleJoin(NetPlayer player)
    {
        string msg = $"[c/74751b:[][c/d9db5a:{PlayerManager.Tracker.Capable.Count()}][c/9fa128:/][c/d9db5a:{Main.maxNetPlayers}][c/74751b:]] [i:1138] [c/167f19:>>>] {player.Name}";
        PlayerUtilities.BroadcastText(msg, new Color(72, 199, 76));
    }

    public void HandleLeave(NetPlayer player)
    {
        string msg = $"[c/74751b:[][c/d9db5a:{PlayerManager.Tracker.Capable.Count()}][c/9fa128:/][c/d9db5a:{Main.maxNetPlayers}][c/74751b:]] [i:1138] [c/7f1616:>>>] {player.Name}";
        PlayerUtilities.BroadcastText(msg, new Color(199, 72, 72));
    }
}
