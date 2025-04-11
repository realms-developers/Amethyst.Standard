using Amethyst.Players;
using Talk.Rendering;

namespace Talk;

public interface IChatProvider
{
    public void HandleJoin(NetPlayer player);
    public void HandleLeave(NetPlayer player);

    public void HandlePlayerMessage(RenderResult renderResult);
}