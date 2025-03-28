using Amethyst.Players;

namespace Amethyst.Talk.Filtering;

public delegate void FilteringHandler(NetPlayer? sender, string text, ref bool ignore);