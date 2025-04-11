using Amethyst.Players;

namespace Talk.Filtering;

public delegate void FilteringHandler(NetPlayer? sender, string text, ref bool ignore);