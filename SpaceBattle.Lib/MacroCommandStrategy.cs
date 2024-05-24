using Hwdtech;

namespace Spacebattle;

public class MacroCommandStrategy : IStrategy
{
    public object Strategy(params object[] args)
    {
        var key = (string)args[0];
        var obj = (IUObject)args[1];

        var dependencies = IoC.Resolve<IList<string>>("SpaceBattle.Operation." + key);

        var resolvedDependencies = dependencies.Select(d => IoC.Resolve<Command.ICommand>(d, obj)).ToList();
        IList<Command.ICommand> list = resolvedDependencies;

        return new MacroCommand(list);
    }
}
