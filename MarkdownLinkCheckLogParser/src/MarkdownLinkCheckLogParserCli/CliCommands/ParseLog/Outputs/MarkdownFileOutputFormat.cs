namespace MarkdownLinkCheckLogParserCli.CliCommands.ParseLog.Outputs;

internal class MarkdownFileOutputFormat : IOutputFormat
{
    private readonly IFile _file;
    private readonly OutputMarkdownFilepath _filepath;

    public MarkdownFileOutputFormat(IFile file, OutputMarkdownFilepath filepath)
    {
        _file = file.NotNull();
        _filepath = filepath.NotNull();
    }

    public async Task WriteAsync(MarkdownLinkCheckOutput output)
    {
        output.NotNull();
        try
        {
            await WriteMarkdownToFileAsync(output);
        }
        catch (Exception e)
        {
            throw new FailedToCreateMarkdownFileException(_filepath, e);
        }
    }

    private async Task WriteMarkdownToFileAsync(MarkdownLinkCheckOutput output)
    {
        using var streamWriter = _file.CreateFileStreamWriter(_filepath);
        await streamWriter.WriteLineAsync("## Markdown link check results");
        await streamWriter.WriteLineAsync();
        if (output.HasErrors)
        {
            await streamWriter.WriteLineAsync(":x: Markdown link check found broken links in your markdown files.");
        }
        else
        {
            await streamWriter.WriteLineAsync(":heavy_check_mark: Markdown link check didn't findany broken links in your markdown files.");
        }

        await streamWriter.WriteLineAsync();
        await streamWriter.WriteLineAsync("| Statistic | Value");
        await streamWriter.WriteLineAsync("| --- | --- |");
        await streamWriter.WriteLineAsync($"| total files checked | {output.TotalFilesChecked}");
        await streamWriter.WriteLineAsync($"| total links checked | {output.TotalLinksChecked}");
        await streamWriter.WriteLineAsync($"| total errors | {output.TotalErrors}");
        await streamWriter.WriteLineAsync();
        await streamWriter.WriteLineAsync("<details>");
        await streamWriter.WriteLineAsync("<summary><strong>Files</strong></summary>");
        for (var i = 0; i < output.Files.Count; i++)
        {
            await streamWriter.WriteLineAsync();
            var checkedFile = output.Files[i];
            await streamWriter.WriteLineAsync($"### {checkedFile.Filename}");
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync($"Links checked: {checkedFile.LinksChecked}");
            await streamWriter.WriteLineAsync($"Errors: {checkedFile.ErrorCount}");
            await streamWriter.WriteLineAsync();
            if (checkedFile.HasErrors)
            {
                await streamWriter.WriteLineAsync("| Link | Status code");
                await streamWriter.WriteLineAsync("| --- | --- |");
                foreach (var (link, statusCode) in checkedFile.Errors)
                {
                    await streamWriter.WriteLineAsync($"| {link} | {statusCode}");
                }

                await streamWriter.WriteLineAsync();
            }

            if (i != output.Files.Count - 1)
            {
                await streamWriter.WriteLineAsync("---");
            }
        }
    }
}