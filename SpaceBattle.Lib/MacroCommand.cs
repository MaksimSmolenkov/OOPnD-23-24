using Command;

namespace SpaceBattle;

public class MacroCommand : ICommand
{
    private readonly IList<ICommand> list;

    public MacroCommand(IList<ICommand> list)
    {
        this.list = list;
    }

    public void Execute()
    {
        list.ToList().ForEach(item => { item.Execute(); });
    }
}
