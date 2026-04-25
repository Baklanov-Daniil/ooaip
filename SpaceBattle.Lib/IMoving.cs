using System.Numerics;


namespace SpaceBattle.Lib;
public interface IMoving
{
    Vectors Position { get; set; }
    Vectors Velocity { get; }
};
