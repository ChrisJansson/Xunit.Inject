using System;

namespace Xunit.Inject.Tests
{
    [RunWith(typeof(TestInjectedTestClassCommand))]
    public class InjectedTestClassCommandTests
    {
        private readonly DependencyA _dependency;

        public InjectedTestClassCommandTests(DependencyA dependency)
        {
            _dependency = dependency;
        }

        [Fact]
        public void Injects_dependencies()
        {
            Assert.NotNull(_dependency);
        }

        public class TestInjectedTestClassCommand : InjectedTestClassCommand
        {
            protected override ITestFixtureFactory GetFactory(Type type)
            {
                var testFixtureFactory = new TestFixtureFactory(type);
                testFixtureFactory.Configure(x => new DependencyA(
                    (DependencyB)x.Create(typeof(DependencyB)),
                    (DependencyC)x.Create(typeof(DependencyC))));
                testFixtureFactory.Configure(() => new DependencyB());
                testFixtureFactory.Configure(() => new DependencyC());

                return testFixtureFactory;
            }
        }

        public class DependencyA
        {
            public DependencyA(DependencyB b, DependencyC c)
            {

            }
        }

        public class DependencyB
        {

        }

        public class DependencyC
        {

        }
    }
}