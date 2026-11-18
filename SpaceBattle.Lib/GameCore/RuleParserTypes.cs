namespace SpaceBattle.Lib;

public sealed class ParsedRule
{
    public string Verb { get; }
    public string[] Args { get; }
    public string RawLine { get; }

    public ParsedRule(string verb, string[] args, string rawLine)
    {
        Verb = verb;
        Args = args;
        RawLine = rawLine;
    }
}

public interface IRuleParser
{
    IReadOnlyList<ParsedRule> ParseFile(string path);
    IReadOnlyList<ParsedRule> Parse(TextReader reader);
}

public sealed class RuleParseException : Exception
{
    public RuleParseException(string message) : base(message) { }
}

