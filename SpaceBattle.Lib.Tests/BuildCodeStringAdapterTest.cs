using Hwdtech;
using Hwdtech.Ioc;
using Movable;
namespace SpaceBattle.Lib.Test;

public class BuildCodeStringAdapterTests
{
    [Fact]
    public void BuildString()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var type = typeof(IMovable);
        var builder = new CodeStringAdapterBuilder(className: "MovableAdapter");

        type.GetProperties().ToList().ForEach(property => builder.AddMember(new
        {
            name = property.Name,
            type = property.PropertyType.Name,
            get = property.CanRead,
            set = property.CanWrite
        }));

        var valid = @"using System;
public class MovableAdapter
{
    private object obj;
    
        private Vector Location{
        
        set
        {
            Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>(""Location.Set"", obj, value).Execute();
        }
          
        get
        
        {
            return Hwdtech.IoC.Resolve<Vector>(""Location.Get"", obj);
        } 
        
        }
        
        private Vector Velosity{
          
        get
        
        {
            return Hwdtech.IoC.Resolve<Vector>(""Velosity.Get"", obj);
        } 
        
        }
        
    public MovableAdapter(object obj)
    {
        this.obj = obj;
    }
}";

        var result = builder.Build();
        Assert.Equal(valid, result);
    }
}
