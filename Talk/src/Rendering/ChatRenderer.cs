using Amethyst.Players;

namespace Talk.Rendering;

public static class ChatRenderer
{
    private static readonly List<RenderingHandler> _handlers = new(8);

    public static IReadOnlyList<RenderingHandler> Handlers => _handlers;

    public static RenderResult RenderMessage(NetPlayer player, string text)
    {
        RenderOperation operation = new(player, text);
        foreach (RenderingHandler handler in _handlers)
        {
            handler(operation);
        }

        string prefix = "";
        foreach (RenderSnippet? pfx in operation.Prefix.OrderBy(p => p.Priority))
        {
            prefix += pfx.Text + " ";
        }

        string suffix = "";
        foreach (RenderSnippet? sfx in operation.Suffix.OrderBy(p => p.Priority))
        {
            suffix += sfx.Text + " ";
        }

        string postfix = "";
        foreach (RenderSnippet? pfx in operation.Postfix.OrderBy(p => p.Priority))
        {
            postfix += pfx.Text + " ";
        }

        string message = string.Format(operation.MessageFormat, prefix, operation.Name, suffix, operation.Text, postfix);

        return new RenderResult(player, message, operation.Color);
    }

    public static void AddRenderer(RenderingHandler handler) => _handlers.Add(handler);
    public static void RemoveRenderer(RenderingHandler handler) => _handlers.RemoveAll(p => p == handler);
}
