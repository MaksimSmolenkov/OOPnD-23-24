namespace SpaceBattle.Lib;

public class MyClass
{
    public IDependency Dependency { get; }

    public MyClass(IDependency dependency)
    {
        Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
    }
}
