using Xunit;
using App;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class TestAuthContext : IAuthContext
{
    public string Token { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
}

public class AuthTests : IDisposable
{
    public AuthTests()
    {
        new App.Scopes.InitCommand().Execute();
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    public void Dispose()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    [Fact]
    public void AuthCommand_WithValidToken_ExecutesWithoutException()
    {
        var context = new TestAuthContext 
        { 
            Token = "valid_token", 
            GameId = "game_1", 
            PlayerId = "player_1" 
        };

        var authCmd = Ioc.Resolve<App.ICommand>("Actions.Auth", context);

        var exception = Record.Exception(() => authCmd.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void AuthCommand_WithEmptyToken_ThrowsException()
    {
        new RegisterIoCDependencyAuth().Execute();

        var context = new TestAuthContext 
        { 
            Token = "", 
            GameId = "game_1", 
            PlayerId = "player_1" 
        };

        var authCmd = Ioc.Resolve<App.ICommand>("Actions.Auth", context);

        Assert.Throws<Exception>(() => authCmd.Execute());
    }
}
