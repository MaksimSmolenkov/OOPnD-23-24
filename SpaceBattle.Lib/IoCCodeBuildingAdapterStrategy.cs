﻿using Hwdtech;

namespace SpaceBattle.Lib
{
    public class IoCObjectCreationStrategy : ICommand
    {
        private readonly string _key;
        private readonly Type _type;

        public IoCObjectCreationStrategy(string key, Type type)
        {
            _key = key;
            _type = type;
        }

        public void Execute()
        {
            IoC.Resolve<ICommand>("IoC.Register", _key, (object[] _) =>
            {
                var constructor = _type.GetConstructors().FirstOrDefault()
                    ?? throw new InvalidOperationException($"No public constructor found for {_type.Name}");

                var resolvedParameters = constructor.GetParameters()
                    .Select(p => IoC.Resolve<object>(p.ParameterType.FullName!))
                    .ToArray();

                return Activator.CreateInstance(_type, resolvedParameters);
            }).Execute();
        }
    }
}
