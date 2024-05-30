using Hwdtech;
using Hwdtech.Ioc;
using Moq;
using Spacebattle;

namespace SpaceBattle.Lib.Test;

public class MacroCommandsTest
{
    public MacroCommandsTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        var commandMock = new Mock<Command.ICommand>();
        commandMock.Setup(c => c.Execute());

        var propertyMock = new Mock<IStrategy>();
        propertyMock.Setup(s => s.Strategy(It.IsAny<object[]>())).Returns(commandMock.Object);

        var listMock = new Mock<IStrategy>();
        listMock.Setup(l => l.Strategy()).Returns(new string[] { "Second" });

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SpaceBattle.Operation.First", (object[] p) => listMock.Object.Strategy(p)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Second", (object[] p) => propertyMock.Object.Strategy(p)).Execute();
    }

     [Fact]
    public static void TestMacroCommand()
    {
        var obj = new Mock<Command.ICommand>();
        obj.Setup(m => m.Execute()).Verifiable();

        var commands = new List<Command.ICommand> { obj.Object };
        var macroCommand = new MacroCommand(commands);

        macroCommand.Execute();
        obj.Verify();
    }

    [Fact]
    public void TestMacroCommandStrategu()
    {
        var obj = new Mock<IUObject>();
        var newmc = new MacroCommandStrategy();

        var macrocommand = (Command.ICommand)newmc.Strategy("First", obj.Object);

        macrocommand.Execute();
        Assert.NotNull(macrocommand);
    }
}
