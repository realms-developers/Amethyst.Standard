using Amethyst.Players;

namespace Amethyst.Talk.Filtering;

public static class ChatFilter
{
    private static List<FilteringHandler> _Handlers = new List<FilteringHandler>(8);

    public static IReadOnlyList<FilteringHandler> Handlers => _Handlers.AsReadOnly();

    public static bool FilterText(NetPlayer? sender, string text)
    {
        if (text.StartsWith('/')) return true;

        bool result = false;
        _Handlers.ForEach(p => p(sender, text, ref result));

        return result;
    }

    public static void AddFilter(FilteringHandler handler) => _Handlers.Add(handler);
    public static void RemoveFilter(FilteringHandler handler) => _Handlers.RemoveAll(p => p == handler);
}