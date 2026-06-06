namespace SpaceBattle.Lib
{
    public interface ICollisionDataGenerator
    {
        IList<int[]> GenerateCollisionData();
        Point GeneratePoint(int range);
    }
}
