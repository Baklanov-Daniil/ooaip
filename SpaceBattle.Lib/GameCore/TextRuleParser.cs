namespace SpaceBattle.Lib;

public sealed class TextRuleParser : IRuleParser
{
    public IReadOnlyList<ParsedRule> ParseFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        using var reader = new StreamReader(path);
        return Parse(reader);
    }

    public IReadOnlyList<ParsedRule> Parse(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        var rules = new List<ParsedRule>();
        string? line;
        var lineNumber = 0;
        while ((line = reader.ReadLine()) is not null)
        {
            lineNumber++;
            var content = line.Trim();
            if (content.Length == 0) continue;
            var tokens = Tokenize(content);
            if (tokens.Count < 2)
                throw new RuleParseException($"Invalid rule syntax at line {lineNumber}; expected '<entity> <VERB> <args>'.");
            var verb = tokens[1];
            var args = new[] { tokens[0] }.Concat(tokens.Skip(2)).ToArray();
            rules.Add(new ParsedRule(verb, args, content));
        }
        return rules.AsReadOnly();
    }

    private static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var i = 0;
        while (i < input.Length)
        {
            if (char.IsWhiteSpace(input[i]) || input[i] == ',')
            {
                i++;
                continue;
            }
            if (input[i] == '[')
            {
                var end = input.IndexOf(']', i);
                if (end < 0)
                    throw new RuleParseException($"Unclosed bracket at position {i}.");
                tokens.Add(input[i..(end + 1)]);
                i = end + 1;
                continue;
            }
            var start = i;
            while (i < input.Length && !char.IsWhiteSpace(input[i]) && input[i] != ',' && input[i] != '[')
                i++;
            tokens.Add(input[start..i]);
        }
        return tokens;
    }
}
