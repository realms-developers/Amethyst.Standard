using Amethyst.Players;
using Amethyst.Talk.Rendering;

namespace Amethyst.Talk.Chat;

public interface IChatProvider
{
    public void HandleJoin(NetPlayer player);
    public void HandleLeave(NetPlayer player);

    public void HandlePlayerMessage(RenderResult renderResult);
}