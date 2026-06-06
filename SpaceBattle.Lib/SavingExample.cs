using System;
using System.Threading.Tasks;
using App;
namespace SpaceBattle.Lib;

public class SavingExample
{
    public static void Generate(string filename, Func<int, int, int, int, bool> collisionFn)
    {
        int collisions = 0;
        int total = 0;
        using (StreamWriter writer = new StreamWriter(filename))
        {
            for (int x = -CollisionMath.MAX_POS; x <= CollisionMath.MAX_POS; x++)
                for (int y = -CollisionMath.MAX_POS; y <= CollisionMath.MAX_POS; y++)
                    for (int vx = -CollisionMath.MAX_VEL; vx <= CollisionMath.MAX_VEL; vx++)
                        for (int vy = -CollisionMath.MAX_VEL; vy <= CollisionMath.MAX_VEL; vy++)
                        {
                            if (vx == 0 && vy == 0) continue;
                            total++;
                            if (collisionFn(x, y, vx, vy))
                            {
                                collisions++;
                                writer.WriteLine($"{x} {y} {vx} {vy}");
                            }
                        }
        }
        Console.WriteLine($"{filename}");
        Console.WriteLine($"Checked: {total}");
        Console.WriteLine($"Collisions: {collisions}");
    }

    public static void Main()
    {
        Generate("collision_circle_circle.txt", CollisionMath.CollideCircleCircle);
        Generate("collision_circle_rectangle.txt", CollisionMath.CollideCircleRectangle);
        Generate("collision_rectangle_rectangle.txt", CollisionMath.CollideRectangleRectangle);
    }
}
