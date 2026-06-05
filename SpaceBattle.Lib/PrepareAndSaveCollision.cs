using System;
using System.IO;
namespace SpaceBattle.Lib;

public static class CollisionMath
{
    public const int MAX_POS = 25;
    public const int MAX_VEL = 5;
    public const double CIRCLE_RADIUS = 1.0;
    public const double RECT_HALF_WIDTH = 1.0;
    public const double RECT_HALF_HEIGHT = 2.0;

    public static double Clamp(double v, double a, double b) => Math.Max(a, Math.Min(v, b));

    public static double PointSegmentDistance(double px, double py, double ax, double ay, double bx, double by)
    {
        double abx = bx - ax;
        double aby = by - ay;
        double apx = px - ax;
        double apy = py - ay;
        double abLenSq = abx * abx + aby * aby;

        if (abLenSq == 0) return Math.Sqrt(apx * apx + apy * apy);

        double t = Clamp((apx * abx + apy * aby) / abLenSq, 0.0, 1.0);
        return Math.Sqrt(Math.Pow(px - (ax + abx * t), 2) + Math.Pow(py - (ay + aby * t), 2));
    }

    public static bool CollideCircleCircle(int x, int y, int vx, int vy)
    {
        double totalRadius = CIRCLE_RADIUS * 2;
        return PointSegmentDistance(0, 0, x, y, x + vx, y + vy) <= totalRadius;
    }

    public static double DistancePointToAabb(double px, double py, double halfW, double halfH)
    {
        double dx = px - Clamp(px, -halfW, halfW);
        double dy = py - Clamp(py, -halfH, halfH);
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static bool CollideCircleRectangle(int x, int y, int vx, int vy)
    {
        const int STEPS = 20;
        for (int i = 0; i <= STEPS; i++)
        {
            double t = (double)i / STEPS;
            if (DistancePointToAabb(x + vx * t, y + vy * t, RECT_HALF_WIDTH, RECT_HALF_HEIGHT) <= CIRCLE_RADIUS)
                return true;
        }
        return false;
    }
    public static bool CollideRectangleRectangle(int x, int y, int vx, int vy)
    {
        double expX = RECT_HALF_WIDTH * 2;
        double expY = RECT_HALF_HEIGHT * 2;

        if (Math.Abs(x) <= expX && Math.Abs(y) <= expY)
        {
            return true;
        }

        if (vx == 0 && Math.Abs(x) > expX) return false;
        if (vy == 0 && Math.Abs(y) > expY) return false;

        double xInvEntry = -expX - x;
        double xInvExit = expX - x;
        double yInvEntry = -expY - y;
        double yInvExit = expY - y;

        double xEntry = (vx == 0) ? double.NegativeInfinity : xInvEntry / vx;
        double xExit = (vx == 0) ? double.PositiveInfinity : xInvExit / vx;
        double yEntry = (vy == 0) ? double.NegativeInfinity : yInvEntry / vy;
        double yExit = (vy == 0) ? double.PositiveInfinity : yInvExit / vy;

        double entryTime = Math.Max(Math.Min(xEntry, xExit), Math.Min(yEntry, yExit));
        double exitTime = Math.Min(Math.Max(xEntry, xExit), Math.Max(yEntry, yExit));

        return entryTime <= exitTime && entryTime >= 0.0 && entryTime <= 1.0;
    }
}
