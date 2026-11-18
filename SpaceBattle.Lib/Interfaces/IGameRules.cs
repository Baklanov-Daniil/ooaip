namespace SpaceBattle.Lib;

/// <summary>A game-independent rule that can be applied to an initialization target.</summary>
public interface IGameRule
{
    string Name { get; }
    string Value { get; }
    void Apply(IGameRuleTarget target);
}

/// <summary>Application boundary implemented by state that accepts parsed rules.</summary>
public interface IGameRuleTarget
{
    void Apply(string name, string value);
}

/// <summary>Parses a text stream or configuration file into applicable rules.</summary>
public interface IRuleParser
{
    IReadOnlyList<IGameRule> Parse(TextReader reader);
    IReadOnlyList<IGameRule> ParseFile(string path);
}

/// <summary>Creates rules for one key; register factories to add formats without changing the parser.</summary>
public interface IRuleFactory
{
    bool CanCreate(string name);
    IGameRule Create(string name, string value);
}

public sealed record GameRule(string Name, string Value) : IGameRule
{
    public void Apply(IGameRuleTarget target) => target.Apply(Name, Value);
}

public sealed class RuleParseException(string message) : FormatException(message);
