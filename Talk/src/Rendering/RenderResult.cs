using Amethyst.Network;
using Amethyst.Players;

namespace Talk.Rendering;

public record class RenderResult(NetPlayer player, string Message, NetColor Color);