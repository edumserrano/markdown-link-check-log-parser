namespace MarkdownLinkCheckLogParserCli.MarkdownLinkCheck;

internal class MarkdownLinkCheckOutput
{
    public MarkdownLinkCheckOutput(IReadOnlyList<MarkdownFileCheck> files, bool captureErrorsOnly)
    {
        files.NotNull();
        Files = captureErrorsOnly
            ? files.Where(x => x.HasErrors).ToList()
            : files;
        TotalFilesChecked = files.Count;
        TotalLinksChecked = files.Sum(x => x.LinksChecked);
        TotalErrors = files.Sum(x => x.ErrorCount);
        HasErrors = TotalErrors > 0;
        FilesWithErrors = files.Count(x => x.HasErrors);
    }

    public int TotalFilesChecked { get; }

    public int TotalLinksChecked { get; }

    public bool HasErrors { get; }

    public int FilesWithErrors { get; }

    public int TotalErrors { get; }

    public IReadOnlyList<MarkdownFileCheck> Files { get; }
}
