using vectr;
namespace SpaceBattle.Lib;

public interface IShootable
{
    string ProjectileType
    {
        get;
        set;
    }
    Vector Pos
    {
        get;
        set;
    }

    Vector Velocity
    {
        get;
    }
}
