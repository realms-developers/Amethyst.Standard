using Amethyst.Network;
using Amethyst.Players;

namespace Amethyst.Talk.Rendering;

public record class RenderResult(NetPlayer player, string Message, NetColor Color);