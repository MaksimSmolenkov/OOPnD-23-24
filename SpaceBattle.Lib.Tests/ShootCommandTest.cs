using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Spacebattle;

namespace SpaceBattle.Lib.Test
{
    public class ShootCommandTests
    {
        public ShootCommandTests()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        }

        [Fact]
        public void ShootCommand_CreatesProjectileWithCorrectProperties_AndStartsMovement()
        {
            var shooterMock = new Mock<IShootable>();
            var gameFieldMock = new Mock<IUObject>();
            var projectileMock = new Mock<IUObject>();
            var moveCommandMock = new Mock<Command.ICommand>();

            var projectileType = "Torpedo";

            shooterMock.SetupGet(s => s.ProjectileType).Returns(projectileType);
            shooterMock.SetupGet(s => s.Pos).Returns((vectr.Vector)It.IsAny<object>());
            shooterMock.SetupGet(s => s.Velocity).Returns((vectr.Vector)It.IsAny<object>());

            Func<string, IUObject> projectileFactory = (type) =>
            {
                Assert.Equal(projectileType, type);
                return projectileMock.Object;
            };

            Action<IUObject, IShootable> projectileConfigurator = (projectile, shooter) =>
            {
                projectile.SetProperty("Location", shooter.Pos);
                projectile.SetProperty("Velocity", shooter.Velocity);
            };

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ShootCommand.ProjectileFactory", (object[] args) => projectileFactory).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ShootCommand.ProjectileConfigurator", (object[] args) => projectileConfigurator).Execute();

            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "StartMove", (object[] args) => moveCommandMock.Object).Execute();

            var shootCommand = new ShootCommand(
                shooterMock.Object,
                gameFieldMock.Object,
                IoC.Resolve<Func<string, IUObject>>("ShootCommand.ProjectileFactory"),
                IoC.Resolve<Action<IUObject, IShootable>>("ShootCommand.ProjectileConfigurator")
            );

            shootCommand.Execute();

            projectileMock.Verify(p => p.SetProperty("Location", It.IsAny<object>()), Times.Once);
            projectileMock.Verify(p => p.SetProperty("Velocity", It.IsAny<object>()), Times.Once);
            gameFieldMock.Verify(g => g.SetProperty("Projectiles", It.IsAny<object>()), Times.Once);

            moveCommandMock.Verify(m => m.Execute(), Times.Once);
        }
    }
}
