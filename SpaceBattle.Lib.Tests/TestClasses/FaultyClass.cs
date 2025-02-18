namespace SpaceBattle.Lib.Test;
public class FaultyClass
{
    public FaultyClass() => throw new Exception("Cannot instantiate");
}
