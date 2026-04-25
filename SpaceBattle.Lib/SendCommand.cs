namespace SpaceBattle.Lib;

public class SendCommand : ICommand
{
    private ICommand _command { get; }
    private ICommandReceiver _receiver { get; }
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
