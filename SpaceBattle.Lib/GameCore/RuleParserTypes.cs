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

/// <summary>Creates <see cref="IGameRule"/> instances for a given rule name.</summary>
public interface IRuleFactory
{
    bool CanCreate(string name);
    IGameRule Create(string name, string value);
}

/// <summary>Parses text rule definitions into <see cref="IGameRule"/> instances.</summary>
public interface IRuleParser
{
    IReadOnlyList<IGameRule> ParseFile(string path);
    IReadOnlyList<IGameRule> Parse(TextReader reader);
}

/// <summary>Thrown when a rule file contains malformed or unknown rules.</summary>
public sealed class RuleParseException : Exception
{
    public RuleParseException(string message) : base(message) { }
}

/// <summary>Default implementation of <see cref="IGameRule"/>.</summary>
public sealed class GameRule : IGameRule
{
    public string Name { get; }
    public string Value { get; }

    public GameRule(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public void Apply(IGameRuleTarget target) => target.Apply(Name, Value);
}
