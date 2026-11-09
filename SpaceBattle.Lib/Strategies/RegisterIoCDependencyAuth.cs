using App;

namespace SpaceBattle.Lib;

public interface IAuthContext
{
    string Token { get; }
    string GameId { get; }
    string PlayerId { get; }
}

public class AuthCommand : App.ICommand
{
    private readonly IAuthContext _context;

    public AuthCommand(IAuthContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        bool isValid = (bool)Ioc.Resolve<bool>("Auth.ValidateToken", _context.Token, _context.GameId, _context.PlayerId);

        if (!isValid)
        {
            throw new Exception("Authorization failed: Invalid token or access denied.");
        }
    }
}

public class RegisterIoCDependencyAuth : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
                "IoC.Register",
                "Actions.Auth",
                (object[] args) => new AuthCommand((IAuthContext)args[0])
            ).Execute();

        Ioc.Resolve<App.ICommand>(
                "IoC.Register",
                "Auth.ValidateToken",
                (object[] args) => {
                    var token = (string)args[0];
                    return (object)!string.IsNullOrEmpty(token);
                }
            ).Execute();
    }
}
