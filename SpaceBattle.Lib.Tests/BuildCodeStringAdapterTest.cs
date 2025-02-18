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
        type.GetProperties().ToList().ForEach((property) => builder.AddMember(new
        {
            name = property.Name,
            type = property.PropertyType.Name,
            get = property.CanRead,
            set = property.CanWrite
        }));
        var t = @"using System;
public class {{name }}
{
    private object obj;
    {{ for property in properties }}
    private {{ property.type }} {{ property.name }}{
    {{if property.set}}
    set
    {
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>(""{{property.name}}.Set"", obj, value).Execute();
    }
    {{end}}  
    get
    {{if property.get}}
    {
        return Hwdtech.IoC.Resolve<{{property.type}}>(""{{property.name}}.Get"", obj);
    } 
    {{ end }}
    }
    {{ end }}
    public {{ name }}(object obj)
    {
        this.obj = obj;
    }
}";
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
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Template", (object[] par) => t).Execute();
        var result = builder.Build();
        Assert.Equal(valid, result);
    }
}
