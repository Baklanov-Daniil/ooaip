using App;
using App.Scopes;
using SpaceBattle.Lib;

namespace SpaceBattle.Lib.Tests;

public class SavingExampleTests
{
    [Fact]
    public void Test_GenerateWritesCollisionsToFile()
    {
        string tempFile = Path.GetTempFileName();        
        Func<int, int, int, int, bool> mockCollisionFn = (x, y, vx, vy) => 
        {
            return x == 1 && y == 1 && vx == 1 && vy == 1;
        };

        try
        {
            SavingExample.Generate(tempFile, mockCollisionFn);
            Assert.True(File.Exists(tempFile));
           
            string[] lines = File.ReadAllLines(tempFile);
            Assert.Contains("1 1 1 1", lines);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void Test_MainGeneratesAllThreeFiles()
    {
        string file1 = "collision_circle_circle.txt";
        string file2 = "collision_circle_rectangle.txt";
        string file3 = "collision_rectangle_rectangle.txt";

        DeleteIfExists(file1);
        DeleteIfExists(file2);
        DeleteIfExists(file3);

        try
        {
            SavingExample.Main();
            Assert.True(File.Exists(file1), "create file");
            Assert.True(File.Exists(file2), "create file");
            Assert.True(File.Exists(file3), "create file");
        }
        finally
        {
            DeleteIfExists(file1);
            DeleteIfExists(file2);
            DeleteIfExists(file3);
        }
    }

    private void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
