namespace MarkdownLinkCheckLogParserCli.MarkdownLinkCheck;

internal class ParserState
{
    private MarkdownFileLog? _current;
    private readonly List<MarkdownFileLog> _logs = new List<MarkdownFileLog>();

    public IReadOnlyList<MarkdownFileLog> Logs => _logs;

    public void VisitStartOfFileSummaryLogLine(StartOfFileSummaryLogLine logLine)
    {
        // if there is no current then set the current.
        // if there's already a current being tracked then it means we need to save
        // it to the parsed logs lines before we start tracking a new current
        if (_current is not null)
        {
            _logs.Add(_current);
        }

        _current = new MarkdownFileLog(logLine.Filename);
    }

    public void VisitLinksCheckedLogLine(LinksCheckedLogLine logLine)
    {
        if (_current is null)
        {
            return;
        }

        _current.LinksChecked = logLine.LinksChecked;
    }

    public void VisitErrorLogLine(ErrorLogLine logLine)
    {
        if (_current is null)
        {
            return;
        }

        _current.AddError(logLine.Link, logLine.StatusCode);
    }

    public void EndOfLog()
    {
        // make sure we don't loose the current item (last log line we parsed)
        if (_current is not null)
        {
            _logs.Add(_current);
            _current = null;
        }
    }
}
