using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace Xunit.Inject
{
    public abstract class InjectedTestClassCommand : TestClassCommand
    {
        protected abstract ITestFixtureFactory GetFactory(Type type);

        public override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo testMethod)
        {
            var factory = GetFactory(TypeUnderTest.Type);
            return base.EnumerateTestCommands(testMethod)
                .Select(x => new InjectedTestCommand(x, factory))
                .ToList();
        }
    }
}