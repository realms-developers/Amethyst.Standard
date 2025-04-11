using Amethyst.Players;

namespace Talk.Filtering;

public static class ChatFilter
{
    private static readonly List<FilteringHandler> _handlers = new(8);

    public static IReadOnlyList<FilteringHandler> Handlers => _handlers.AsReadOnly();

    public static bool FilterText(NetPlayer? sender, string text)
    {
        if (text.StartsWith('/'))
        {
            return true;
        }

        bool result = false;
        _handlers.ForEach(p => p(sender, text, ref result));

        return result;
    }

    public static void AddFilter(FilteringHandler handler) => _handlers.Add(handler);
    public static void RemoveFilter(FilteringHandler handler) => _handlers.RemoveAll(p => p == handler);
}
