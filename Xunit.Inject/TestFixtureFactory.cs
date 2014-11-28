using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xunit.Inject
{
    public class TestFixtureFactory : ITestFixtureFactory
    {
        private readonly IDictionary<Type, Func<TestFixtureFactoryContext, object>> _creators = new Dictionary<Type, Func<TestFixtureFactoryContext, object>>();
        private readonly Type _type;

        public TestFixtureFactory(Type type)
        {
            _type = type;
        }

        public void Configure<T>(Func<T> creator)
        {
            Configure(x => creator());
        }

        public void Configure<T>(Func<TestFixtureFactoryContext, T> creator)
        {
            Func<TestFixtureFactoryContext, object> castedCreator = x => creator(x);
            _creators.Add(typeof(T), castedCreator);
        }

        public ITestFixtureScope Create()
        {
            var context = new TestFixtureFactoryContext(this);

            var constructor = GetConstructor(_type);

            var constructorParameters = constructor.GetParameters()
                .Select(x => context.Create(x.ParameterType))
                .ToArray();

            var instance = constructor.Invoke(constructorParameters);
            return new TestFixtureScope(instance, context.GetDisposables());
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors();
            if (constructors.Length > 1)
            {
                throw new NotSupportedException("Multiple constructors are not supported");
            }
            return constructors.Single();
        }

        public class TestFixtureFactoryContext
        {
            private readonly TestFixtureFactory _factory;
            private readonly IDictionary<Type, object> _instances = new Dictionary<Type, object>();

            public TestFixtureFactoryContext(TestFixtureFactory factory)
            {
                _factory = factory;
            }

            public object Create(Type type)
            {
                if (_instances.ContainsKey(type))
                {
                    return _instances[type];
                }

                var instance = _factory._creators[type](this);
                _instances[type] = instance;
                return instance;
            }

            public IEnumerable<IDisposable> GetDisposables()
            {
                return _instances
                    .Select(x => x.Value)
                    .OfType<IDisposable>()
                    .ToList();
            }
        }

        private class TestFixtureScope : ITestFixtureScope
        {
            private readonly IEnumerable<IDisposable> _disposables;

            public TestFixtureScope(object instance, IEnumerable<IDisposable> disposables)
            {
                _disposables = disposables;
                Fixture = instance;
            }

            public object Fixture { get; private set; }

            public void Dispose()
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}