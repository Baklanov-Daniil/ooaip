namespace SpaceBattle.Lib;

public class SendCommand : ICommand
{
<<<<<<< HEAD
    private ICommand _command { get; }
    private ICommandReceiver _receiver { get; }
=======
    private ICommand _command {get;}
    private ICommandReceiver _receiver {get;}
>>>>>>> 0fe2ab7 (final)
    public SendCommand(ICommand command, ICommandReceiver receiver)
    {
        _command = command;
        _receiver = receiver;
    }

    public void Execute()
    {
        _receiver.Receive(_command);
    }
}
