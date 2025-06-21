using Amethyst.Systems.Commands.Base;
using Amethyst.Systems.Commands.Dynamic.Attributes;
using Amethyst.Systems.Users.Base;
using Amethyst.Text;
using static Handbook.Handbook;

namespace Handbook.Commands;

public static partial class Commands
{
    private const string _seePerm = "handbook.see";

    [Command("handbook list", "handbook.list.desc")]
    [CommandPermission(_seePerm)]
    [CommandSyntax("en-US", "[page]")]
    [CommandSyntax("ru-RU", "[страница]")]
    public static void List(IAmethystUser _, CommandInvokeContext ctx, int page = 0)
    {
        // Get all text files in the _handbookBasePath directory
        IEnumerable<string> documents;

        try
        {
            documents = Directory.GetFiles(_handbookBasePath, "*.txt")
                .Select(f => Path.GetFileNameWithoutExtension(f) ?? "?");
        }
        catch
        {
            ctx.Messages.ReplyError("commands.noAvailablePages");
            return;
        }

        // Check if empty
        if (!documents.Any())
        {
            ctx.Messages.ReplyError("commands.noAvailablePages");
            return;
        }

        // Create pages and display to user
        PagesCollection pages = PagesCollection.AsPage(documents);

        ctx.Messages.ReplyPage(pages, "handbook.list.header", null, null, false, page);
    }

    [Command("handbook see", "handbook.see.desc")]
    [CommandPermission(_seePerm)]
    [CommandSyntax("en-US", "<document>", "[page]")]
    [CommandSyntax("ru-RU", "<документ>", "[страница]")]
    public static void See(IAmethystUser _, CommandInvokeContext ctx, string doc, int page = 0)
    {
        string docPath = Path.Combine(_handbookBasePath, doc);

        // Try to get the file without extension, expecting only .txt files
        if (!docPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            docPath += ".txt";
        }

        // Check if the file exists
        if (!File.Exists(docPath))
        {
            ctx.Messages.ReplyError("commands.objectNotFound");
            return;
        }

        // Read the file content
        string docContent;
        try
        {
            docContent = File.ReadAllText(docPath);
        }
        catch
        {
            ctx.Messages.ReplyError("commands.objectNotFound");
            return;
        }

        // Create pages and display to user
        PagesCollection pages = PagesCollection.AsPage(docContent.Split(_hCfg.Data.NewLine), _hCfg.Data.MaxLines);

        ctx.Messages.ReplyPage(pages, null, null, null, true, page);
    }
}