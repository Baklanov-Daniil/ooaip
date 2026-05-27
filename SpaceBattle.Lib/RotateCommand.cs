namespace SpaceBattle.Lib;
public class RotateCommand: ICommand
{
    private readonly IRotating rotating;

    public RotateCommand(IRotating rotating)
    {
        this.rotating = rotating;
    }

    public void Execute()
    {
          rotating.CurrentAngle += rotating.AngleSpeed;
    }
}
