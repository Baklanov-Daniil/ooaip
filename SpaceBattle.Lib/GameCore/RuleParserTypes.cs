namespace SpaceBattle.Lib;

public sealed class ParsedRule
{
    public string Verb { get; }
    public string[] Args { get; }

    public ParsedRule(string verb, string[] args)
    {
        Verb = verb;
        Args = args;
    }
}

public sealed class RuleParseException : Exception
{
    public RuleParseException(string message) : base(message) { }
}

