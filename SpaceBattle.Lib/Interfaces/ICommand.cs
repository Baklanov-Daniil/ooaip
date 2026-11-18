namespace SpaceBattle.Lib;
public interface ICommand
{
    void Execute();
}

public interface IDescribedCommand : ICommand
{
    string Description { get; }
}
