namespace MarkdownLinkCheckLogParserCli.MarkdownLinkCheck.LogLines;

internal static class MarkdownLinkCheckLogLineFactory
{
    public static IMarkdownLinkCheckLogLine Create(ReadOnlyMemory<char> line)
    {
        (var isStartOfFileSummary, var filename) = IsStartOfFileSummary(line.Span);
        if (isStartOfFileSummary)
        {
            return new StartOfFileSummaryLogLine(filename);
        }

        (var isLinksChecked, var linksChecked) = IsLinksCheckedLine(line.Span);
        if (isLinksChecked)
        {
            return new LinksCheckedLogLine(linksChecked);
        }

        if (IsErrorLine(line.Span, out var link, out var statusCode))
        {
            return new ErrorLogLine(link, statusCode);
        }

        return UnknownLogLine.Instance;
    }

    private static (bool IsStartOfFile, string Filename) IsStartOfFileSummary(ReadOnlySpan<char> line)
    {
        // example line:
        // FILE: ./tests/liquid-test-logger-template.md
        const string marker = "FILE: ";
        var startOfFileSummaryIdx = line.IndexOf(marker, StringComparison.InvariantCulture);
        if (startOfFileSummaryIdx >= 0)
        {
            var filename = line
                .Slice(startOfFileSummaryIdx + marker.Length)
                .Trim()
                .ToString();
            return (true, filename);
        }

        return (false, string.Empty);
    }

    private static (bool IsLinksChecked, int LinksChecked) IsLinksCheckedLine(ReadOnlySpan<char> line)
    {
        // example line:
        // 0 links checked.
        const string marker = " links checked.";
        var linksCheckedIdx = line.IndexOf(marker, StringComparison.InvariantCulture);
        if (linksCheckedIdx >= 0)
        {
            var endOfLinesCheckedIdx = line.Length - marker.Length;
            var linesCheckedSpan = line
                .Slice(0, endOfLinesCheckedIdx)
                .Trim();
            if (int.TryParse(linesCheckedSpan, out var linesChecked))
            {
                return (true, linesChecked);
            }
        }

        return (false, -1);
    }

    private static bool IsErrorLine(
        ReadOnlySpan<char> line,
        out string link,
        out int statusCode)
    {
        // example line:
        // [✖] https://github.com/edumserrano/dotnet-sdk-extensions/actions/workflows/pr-dotnet-format-check.yml/badge.svg → Status: 404
        const string errorMarker = "[✖] ";
        const string statusCodeMarker = " → Status: ";
        var errorIdx = line.IndexOf(errorMarker, StringComparison.InvariantCulture);
        var statusCodeIdx = line.IndexOf(statusCodeMarker, StringComparison.InvariantCulture);
        if (errorIdx >= 0 && statusCodeIdx >= 0)
        {
            var range = new Range(start: errorIdx + errorMarker.Length, end: statusCodeIdx);
            var linkSpan = line[range].Trim();
            var statusSpan = line
                .Slice(statusCodeIdx + statusCodeMarker.Length)
                .Trim();
            if (int.TryParse(statusSpan, out statusCode))
            {
                link = linkSpan.ToString();
                return true;
            }
        }

        link = string.Empty;
        statusCode = -1;
        return false;
    }
}
