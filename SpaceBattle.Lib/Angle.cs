using System.Formats.Asn1;
using System.Threading.Channels;
namespace SpaceBattle.Lib;

public class Angle
{
    public static readonly int denominator = 8;
    public int numenator { get; }

    public Angle(int n)
    {
        numenator = n % denominator;
    }

    public static Angle operator +(Angle a1, Angle a2)
    {
            int result = (a1.numenator + a2.numenator) % denominator;
            return new Angle(result);
    }
    

    public static implicit operator double(Angle angle) 
    { 
        return (double)angle.numenator / denominator * 2 * Math.PI; 
    } 

    public static bool operator ==(Angle a1, Angle a2)
    {
        if (a1.numenator != a2.numenator) return false;

        return true;  
    }

        public static bool operator !=(Angle a1, Angle a2)
    {
        if (a1.numenator == a2.numenator) return false;

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

       if (obj is Angle other)
        {
            return this.numenator == other.numenator;
        };

        return false;
    }

    public override int GetHashCode()
    {
        return numenator.GetHashCode();
    }
        
}
