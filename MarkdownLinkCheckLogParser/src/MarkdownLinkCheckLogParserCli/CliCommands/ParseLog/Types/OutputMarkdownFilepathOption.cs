namespace MarkdownLinkCheckLogParserCli.CliCommands.ParseLog.Types;

internal sealed class OutputMarkdownFilepathOption
{
    public OutputMarkdownFilepathOption(string markdownFilePathOption)
    {
        Value = markdownFilePathOption.NotNull();
    }

    public string Value { get; }

    public static implicit operator string(OutputMarkdownFilepathOption filepath)
    {
        return filepath.Value;
    }

    public override string ToString() => (string)this;
}
