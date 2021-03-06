namespace MarkdownLinkCheckLogParserCli.GitHub.Types;

internal sealed class GitHubRepository
{
    private readonly string _value;

    public GitHubRepository(string gitHubRepository)
    {
        _value = gitHubRepository.NotNullOrWhiteSpace();
    }

    public static implicit operator string(GitHubRepository gitHubAuthToken)
    {
        return gitHubAuthToken._value;
    }

    public override string ToString() => (string)this;
}
