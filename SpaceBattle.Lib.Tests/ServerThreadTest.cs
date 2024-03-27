using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Spacebattle;

public class ServerThreadTest
{
    public ServerThreadTest()
    {

        new InitScopeBasedIoCImplementationCommand().Execute();

        var setScopeCommand = IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Root")));

        IoC.Resolve<ICommand>("IoC.Register",
            "Hard Stop The Thread",
            (object[] args) =>
            {
                var thread = (ServerThread)args[0];
                var action = (Action)args[1];
                return new ActionCommand(
                    () =>
                    {
                        new ServerThread.HardStopCommand(thread).Execute();
                        new ActionCommand(action).Execute();
                    }
                );
            }
        ).Execute();

    }
    [Fact]
    public void HardStopShouldStopServerThread()
    {
        var setScopeCommand = IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New",
                IoC.Resolve<object>("Scopes.Current")));

        var mre = new ManualResetEvent(false);

        var q = new BlockingCollection<ICommand>(10);

        var HandleCommand = new Mock<ICommand>();
        HandleCommand.Setup(m => m.Execute()).Verifiable();

        var setRegisterCommand = IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExceptionHandler.Handle", (object[] args) => HandleCommand.Object);

        q.Add(setScopeCommand);
        q.Add(setRegisterCommand);

        var command = new Mock<ICommand>();
        command.Setup(m => m.Execute()).Verifiable();

        var st = new ServerThread(q);
        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });
        q.Add(command.Object);
        q.Add(hs);
        q.Add(command.Object);

        st.Start();

        Assert.Single(q);
    }
}

public class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}
