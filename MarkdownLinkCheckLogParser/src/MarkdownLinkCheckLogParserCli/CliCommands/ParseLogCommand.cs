using MarkdownLinkCheckLogParserCli.MarkdownLinkCheck;

namespace MarkdownLinkCheckLogParserCli.CliCommands;

[Command("parse-log")]
public class ParseLogCommand : ICommand
{
    private readonly HttpClient _httpClient;

    public ParseLogCommand()
        : this(new HttpClient())
    {
    }

    public ParseLogCommand(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [CommandOption(
        "auth-token",
        't',
        IsRequired = true,
        Validators = new Type[] { typeof(NotNullOrWhitespaceOptionValidator) },
        Description = "GitHub token used to access workflow run logs.")]
    public string AuthToken { get; init; } = default!;

    [CommandOption(
        "repo",
        'r',
        IsRequired = true,
        Validators = new Type[] { typeof(NotNullOrWhitespaceOptionValidator) },
        Description = "The repository for the workflow run in the format of {owner}/{repo}.")]
    public string Repo { get; init; } = default!;

    [CommandOption(
        "run-id",
        'i',
        IsRequired = true,
        Validators = new Type[] { typeof(NotNullOrWhitespaceOptionValidator) },
        Description = "The unique identifier of the workflow run that contains the markdown link check step.")]
    public string RunId { get; init; } = default!;

    [CommandOption(
        "job-name",
        'j',
        IsRequired = true,
        Validators = new Type[] { typeof(NotNullOrWhitespaceOptionValidator) },
        Description = "The name of the job that contains the markdown link check step.")]
    public string JobName { get; init; } = default!;

    [CommandOption(
        "step-name",
        's',
        IsRequired = true,
        Validators = new Type[] { typeof(NotNullOrWhitespaceOptionValidator) },
        Description = "The name of the markdown link check step.")]
    public string StepName { get; init; } = default!;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        try
        {
            console.NotNull();
            AuthToken.NotNullOrWhiteSpace();
            Repo.NotNullOrWhiteSpace();
            RunId.NotNullOrWhiteSpace();
            JobName.NotNullOrWhiteSpace();
            StepName.NotNullOrWhiteSpace();

            var gitHubHttpClient = new GitHubHttpClient(_httpClient, AuthToken);
            var gitHubWorkflowRunLogs = new GitHubWorkflowRunLogs(gitHubHttpClient);
            var stepLogLines = await gitHubWorkflowRunLogs.GetWorkflowRunLogForStepAsync(Repo, RunId, JobName, StepName);
            var parsed = MarkdownLinkCheckOutputParser.Parse(stepLogLines);
            var b = "2";

            // var yamlTemplateAsString = await File.ReadAllTextAsync(RunId);
            // var issueFormBodyText = new IssueFormBodyText(AuthToken);
            // var issueFormTemplateText = new IssueFormYamlTemplateText(yamlTemplateAsString);
            // var issueFormTemplate = IssueFormYamlTemplateParser.Parse(issueFormTemplateText);
            // var issueFormBody = IssueFormBodyParser.Parse(issueFormBodyText, issueFormTemplate);
            // var issueFormBodyAsJson = issueFormBody.ToJson();
            // await console.Output.WriteLineAsync(issueFormBodyAsJson);
        }
        catch (Exception e)
        {
            var message = @$"An error occurred trying to execute the command to parse the log from a Markdown link check step.
Error:
- {e.Message}";
            throw new CommandException(message, innerException: e);
        }
    }
}
