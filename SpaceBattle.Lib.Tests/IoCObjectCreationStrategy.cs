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
                (object[] args) => dependencyMock.Object
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
    }

    public class MyClass
    {
        public IDependency Dependency { get; }

        public MyClass(IDependency dependency)
        {
            Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
        }
    }
}
