using Xunit;

namespace SpaceBattle.Lib.Tests;

public class RuleParserTests
{
    [Fact]
    public void ParserCreatesRulesFromSpawnLines()
    {
        var parser = new TextRuleParser();
        var rules = parser.Parse(new StringReader("player1 SPAWN Spaceship [1, 1]\n"));

        Assert.Single(rules);
        Assert.Equal("SPAWN", rules[0].Verb);
        Assert.Equal(["player1", "Spaceship", "[1, 1]"], rules[0].Args);
    }

    [Fact]
    public void ParserCreatesRulesFromCanLines()
    {
        var parser = new TextRuleParser();
        var rules = parser.Parse(new StringReader("Spaceship CAN MoveForward, Rotate, Fire\n"));

        Assert.Single(rules);
        Assert.Equal("CAN", rules[0].Verb);
        Assert.Equal(["Spaceship", "MoveForward", "Rotate", "Fire"], rules[0].Args);
    }

    [Fact]
    public void ParserCreatesRulesFromActionLines()
    {
        var parser = new TextRuleParser();
        var rules = parser.Parse(new StringReader("Spaceship ACTION MoveForward INCLUDE Move, BurnFuel\n"));

        Assert.Single(rules);
        Assert.Equal("ACTION", rules[0].Verb);
        Assert.Equal(["Spaceship", "MoveForward", "INCLUDE", "Move", "BurnFuel"], rules[0].Args);
    }

    [Fact]
    public void ParserReportsMalformedLines()
    {
        var parser = new TextRuleParser();
        var error = Assert.Throws<RuleParseException>(() => parser.Parse(new StringReader("not-a-rule\n")));
        Assert.Contains("line 1", error.Message);
    }

    [Fact]
    public void ParserReportsMissingFiles()
    {
        var parser = new TextRuleParser();
        Assert.Throws<FileNotFoundException>(() => parser.ParseFile(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt")));
    }

    [Fact]
    public void ParserParsesRulesFromGameRulesFile()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "game-rules.txt");
        var parser = new TextRuleParser();
        var rules = parser.ParseFile(path);

        Assert.Equal(6, rules.Count);
        Assert.Equal("SPAWN", rules[0].Verb);
        Assert.Equal("SPAWN", rules[1].Verb);
        Assert.Equal("CAN", rules[2].Verb);
        Assert.Equal("CAN", rules[3].Verb);
        Assert.Equal("ACTION", rules[4].Verb);
        Assert.Equal(["Meteor", "MoveForward", "INCLUDE", "Move"], rules[5].Args);
    }

}
