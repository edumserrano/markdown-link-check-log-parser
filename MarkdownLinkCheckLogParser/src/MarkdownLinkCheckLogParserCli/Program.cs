namespace MarkdownLinkCheckLogParserCli;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var app = new MlcLogParserCli();
        return await app.RunAsync(args);
    }
}
