using Hwdtech;
using Spacebattle;

namespace SpaceBattle.Lib
{
    public class AddProjectileToFieldCommand : Command.ICommand
    {
        private readonly IUObject _gameField;
        private readonly IUObject _projectile;

        public AddProjectileToFieldCommand(IUObject gameField, IUObject projectile)
        {
            _gameField = gameField;
            _projectile = projectile;
        }

        public void Execute()
        {
            var projectiles = _gameField.GetProperty("Projectiles") as List<IUObject> ?? new List<IUObject>();
            projectiles.Add(_projectile);
            _gameField.SetProperty("Projectiles", projectiles);
        }
    }

    public class ShootCommand : Command.ICommand
    {
        private readonly IShootable _shooter;
        private readonly IUObject _gameField;
        private readonly Func<string, IUObject> _projectileFactory;
        private readonly Action<IUObject, IShootable> _projectileConfigurator;

        public ShootCommand(IShootable shooter, IUObject gameField, Func<string, IUObject> projectileFactory, Action<IUObject, IShootable> projectileConfigurator)
        {
            _shooter = shooter;
            _gameField = gameField;
            _projectileFactory = projectileFactory;
            _projectileConfigurator = projectileConfigurator;
        }

        public void Execute()
        {
            var projectile = _projectileFactory.Invoke(_shooter.ProjectileType);

            _projectileConfigurator.Invoke(projectile, _shooter);

            new AddProjectileToFieldCommand(_gameField, projectile).Execute();
            var startMoveCommand = IoC.Resolve<Command.ICommand>("StartMove", projectile);
            startMoveCommand.Execute();
        }
    }
}
