using Moq;
using vectr;
using Hwdtech.Ioc;
using Hwdtech;
using Movable;

namespace SpaceBattle.Lib.Test
{

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
    
        public Vector Location{
        
        set
        {
            Hwdtech.IoC.Resolve<Command.ICommand>(""Location.Set"", obj, value).Execute();
        }
          
        get
        
        {
            return Hwdtech.IoC.Resolve<Vector>(""Location.Get"", obj);
        } 
        
        }
        
        public Vector Velosity{
          
        get
        
        {
            return Hwdtech.IoC.Resolve<Vector>(""Velosity.Get"", obj);
        } 
        
        }
        
    public MovableAdapter(object obj)
    {
        this.obj = obj;
    }
}
return typeof(MovableAdapter);";

        var result = builder.Build();
        Assert.Equal(valid, result);

    }
         [Fact]
        public void BuildString_CreateMock_AndCheckProperties()
        {
            var builder = new CodeStringAdapterBuilder("MovableAdapter");
            builder.AddMember(new { name = "Location", type = "Vector", get = true, set = true })
                   .AddMember(new { name = "Velocity", type = "Vector", get = true, set = false });

            var generatedCode = builder.Build();

            var mockAdapter = new Mock<IMovable>();

            var location = new Vector(1, 2);
            var velocity = new Vector(3, 4);

            mockAdapter.SetupProperty(a => a.Location, location);
            mockAdapter.SetupGet(a => a.Location).Returns(location);

            mockAdapter.SetupGet(a => a.Velosity).Returns(velocity);

            var adapterInstance = mockAdapter.Object;

            Assert.Equal(location, adapterInstance.Location);
            Assert.Equal(velocity, adapterInstance.Velosity);

            mockAdapter.VerifyGet(a => a.Location, Times.Once);
            mockAdapter.VerifyGet(a => a.Velosity, Times.Once);
        }
    }
}
