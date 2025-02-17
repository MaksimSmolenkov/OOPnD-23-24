using System.Reflection;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Test
{
    public class IoCObjectCreationStrategyTests
    {
        public IoCObjectCreationStrategyTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<ICommand>("Scopes.Current.Set",
                IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

        [Fact]
        public void IoCObjectCreationStrategy_RegistersObjectCreationCorrectly()
        {
            var dependencyMock = new Mock<IDependency>();
            dependencyMock.Setup(d => d.DoWork()).Returns("Success");

            IoC.Resolve<ICommand>(
                "IoC.Register",
                typeof(IDependency).FullName!,
                (object[] _) => dependencyMock.Object
            ).Execute();

            var strategy = new IoCObjectCreationStrategy("CreateMyClass", typeof(MyClass));
            strategy.Execute();

            var createdObject = IoC.Resolve<object>("CreateMyClass", Array.Empty<object>());

            Assert.NotNull(createdObject);
            Assert.IsType<MyClass>(createdObject);

            var myClassInstance = (MyClass)createdObject;
            Assert.Equal("Success", myClassInstance.Dependency.DoWork());

            dependencyMock.Verify(d => d.DoWork(), Times.Once);
        }

        [Fact]
        public void IoCObjectCreationStrategy_ThrowsException_WhenDependencyNotRegistered()
        {
            var strategy = new IoCObjectCreationStrategy("CreateClassWithDependency", typeof(MyClass));
            strategy.Execute();

            var ex = Assert.Throws<ArgumentException>(() =>
                IoC.Resolve<object>("CreateClassWithDependency", Array.Empty<object>())
            );
            Assert.Contains("Unknown IoC dependency key", ex.Message);
        }

        [Fact]
        public void IoCObjectCreationStrategy_ThrowsException_WhenActivatorFails()
        {
            var strategy = new IoCObjectCreationStrategy("CreateFaultyClass", typeof(FaultyClass));
            strategy.Execute();

            var ex = Assert.Throws<TargetInvocationException>(() =>
                IoC.Resolve<object>("CreateFaultyClass", Array.Empty<object>())
            );
            Assert.Contains("Cannot instantiate", ex.InnerException?.Message);
        }
    }

    public class MyClass
    {
        public IDependency Dependency { get; }

        public MyClass(IDependency dependency)
        {
            Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
        }
    }

    public class ClassWithoutConstructor
    {
        private ClassWithoutConstructor() { } 
    }

    public class FaultyClass
    {
        public FaultyClass() => throw new Exception("Cannot instantiate");
    }

    public interface IDependency
    {
        string DoWork();
    }
}
