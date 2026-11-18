using System;
using System.IO;
using Xunit;

namespace SpaceBattle.Lib.Tests;

public class RuleParserTests
{
    [Fact]
    public void ParserCreatesRulesThroughRegisteredHandlers()
    {
        var parser = new TextRuleParser(new IRuleFactory[] { new BuiltInRuleFactory() });
        var rules = parser.Parse(new StringReader("resource.max=5\n"));

        Assert.Single(rules);
        Assert.Equal("resource.max", rules[0].Name);
        Assert.Equal("5", rules[0].Value);
    }

    [Fact]
    public void ParserReportsMalformedLines()
    {
        var parser = new TextRuleParser(new IRuleFactory[] { new BuiltInRuleFactory() });
        var error = Assert.Throws<RuleParseException>(() => parser.Parse(new StringReader("not-a-rule")));
        Assert.Contains("line 1", error.Message);
    }

    [Fact]
    public void ParserReportsMissingFiles()
    {
        var parser = new TextRuleParser(new IRuleFactory[] { new BuiltInRuleFactory() });
        Assert.Throws<FileNotFoundException>(() => parser.ParseFile(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt")));
    }

}
