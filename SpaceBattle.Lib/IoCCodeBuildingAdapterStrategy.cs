using Hwdtech;

namespace SpaceBattle.Lib
{
    public class IoCObjectCreationStrategy : ICommand
    {
        private readonly string _key;
        private readonly Type _type;

        public IoCObjectCreationStrategy(string key, Type type)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public void Execute()
        {
            IoC.Resolve<ICommand>("IoC.Register", _key, (object[] _) =>
            {
                var constructor = _type.GetConstructors().FirstOrDefault();

                var resolvedParameters = constructor.GetParameters()
                    .Select(p => IoC.Resolve<object>(p.ParameterType.FullName!))
                    .ToArray();

                return Activator.CreateInstance(_type, resolvedParameters);
            }).Execute();
        }
    }
}
