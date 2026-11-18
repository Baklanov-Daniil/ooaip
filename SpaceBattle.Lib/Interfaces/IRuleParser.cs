namespace SpaceBattle.Lib;

public interface IRuleParser
{
    IReadOnlyList<ParsedRule> ParseFile(string path);
    IReadOnlyList<ParsedRule> Parse(TextReader reader);
}
