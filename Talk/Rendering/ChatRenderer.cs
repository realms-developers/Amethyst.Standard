using Amethyst.Core;
using Amethyst.Players;

namespace Amethyst.Talk.Rendering;

public static class ChatRenderer
{
    private static List<RenderingHandler> _Handlers = new List<RenderingHandler>(8);

    public static IReadOnlyList<RenderingHandler> Handlers => _Handlers;

    public static RenderResult RenderMessage(NetPlayer player, string text)
    {
        RenderOperation operation = new RenderOperation(player, text);
        foreach (var handler in _Handlers)
            handler(operation);

        string prefix = "";
        foreach (var pfx in operation.Prefix.OrderBy(p => p.Priority))
            prefix += pfx.Text + " ";

        string suffix = "";
        foreach (var sfx in operation.Suffix.OrderBy(p => p.Priority))
            suffix += sfx.Text + " ";

        string postfix = "";
        foreach (var pfx in operation.Postfix.OrderBy(p => p.Priority))
            postfix += pfx.Text + " ";

        string message = string.Format(operation.MessageFormat, prefix, operation.Name, suffix, operation.Text, postfix);

        return new RenderResult(player, message, operation.Color);
    }

    public static void AddRenderer(RenderingHandler handler) => _Handlers.Add(handler);
    public static void RemoveRenderer(RenderingHandler handler) => _Handlers.RemoveAll(p => p == handler);
}