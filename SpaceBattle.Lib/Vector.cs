using System.Net;
using System.Linq;

namespace SpaceBattle.Lib;

public class Vectors
{
    public int[] coords { get; }
    public int dimension => coords.Length;

    public Vectors(int[] coords)
    {
        if (coords == null)
        {
            throw new ArgumentNullException(nameof(coords), "координаты не инициализированы!");
        }
                if (coords.Length == 0)
        {
            throw new ArgumentException("координаты не могут быть пустыми!");
        }

        this.coords = coords;     
    }

   public static Vectors operator +(Vectors vec1, Vectors vec2)
    {
        if (vec1.dimension != vec2.dimension)
        {
             throw new ArgumentException("размерности должны совпадать!");
        }

        else
        {
            int[] result = new int[vec1.dimension];
            for(int i = 0; i < vec1.dimension; i++)
            {
                result[i] = vec1.coords[i] + vec2.coords[i];
            }
            return new Vectors(result);
        }
        
    }
    public override bool Equals(object? obj)
    {
        if (obj is not Vectors other)
            return false;

        return coords.SequenceEqual(other.coords);
    }

    public static bool operator ==(Vectors vec1, Vectors vec2)
    {
        if (ReferenceEquals(vec1, vec2))
            return true;

        if (vec1 is null || vec2 is null) return false;
            

        return vec1.coords.SequenceEqual(vec2.coords);
    }

    public static bool operator !=(Vectors vec1, Vectors vec2)
    {
        return !(vec1 == vec2);
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        foreach (var coord in coords)
        {
            hash.Add(coord);
        }
        return hash.ToHashCode();
    }
};
