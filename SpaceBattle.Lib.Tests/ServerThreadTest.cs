using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

public class ServerThreadTest
{
    public ServerThreadTest()
    {

        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("IoC.Register",
            "Hard Stop The Thread",
            (object[] args) =>
            {
                var thread = (ServerThread)args[0];
                var action = (Action)args[1];
                return new ActionCommand(
                    () =>
                    {
                        new HardStopCommand(thread).Execute();
                        new ActionCommand(action).Execute();
                    }
                );
            }
        ).Execute();

    }
    [Fact]
    public void HardStopShouldStopServerThread()
    {
        var q = new BlockingCollection<ICommand>(10);

        var st = new ServerThread(q);

        var command = new Mock<ICommand>();
        command.Setup(m => m.Execute()).Verifiable();

        q.Add(command.Object);
        var mre = new ManualResetEvent(false);
        q.Add(IoC.Resolve<ICommand>("Hard Stop The Thread", st, () => { mre.Set(); }));
        q.Add(command.Object);

        st.Start();
        mre.WaitOne();

        Assert.Single(q);
        command.Verify(m => m.Execute(), Times.Once);
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
