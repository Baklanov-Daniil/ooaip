namespace SpaceBattle.Lib;

/// <summary>One rule syntax extension; provide additional factories without editing the parser.</summary>
public sealed class BuiltInRuleFactory : IRuleFactory
{
    public bool CanCreate(string name) => name.StartsWith("resource.", StringComparison.Ordinal)
        || name is "victory.condition" or "damage.multiplier" or "movement.multiplier";

    public IGameRule Create(string name, string value) => new GameRule(name, value);
}

/// <summary>Parses key=value lines and delegates rule construction to registered factories.</summary>
public sealed class TextRuleParser : IRuleParser
{
    private readonly IReadOnlyList<IRuleFactory> _factories;

    public TextRuleParser(IEnumerable<IRuleFactory> factories) => _factories = factories.ToArray();

    public IReadOnlyList<IGameRule> ParseFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        using var reader = new StreamReader(path);
        return Parse(reader);
    }

    public IReadOnlyList<IGameRule> Parse(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        var rules = new List<IGameRule>();
        string? line;
        var lineNumber = 0;
        while ((line = reader.ReadLine()) is not null)
        {
            lineNumber++;
            var content = line.Split('#', 2)[0].Trim();
            if (content.Length == 0) continue;
            var separator = content.IndexOf('=');
            if (separator <= 0 || separator == content.Length - 1)
                throw new RuleParseException($"Invalid rule syntax at line {lineNumber}; expected name=value.");
            var name = content[..separator].Trim();
            var value = content[(separator + 1)..].Trim();
            var factory = _factories.FirstOrDefault(candidate => candidate.CanCreate(name));
            if (factory is null)
                throw new RuleParseException($"Unknown rule '{name}' at line {lineNumber}.");
            rules.Add(factory.Create(name, value));
        }
        return rules.AsReadOnly();
    }
}
