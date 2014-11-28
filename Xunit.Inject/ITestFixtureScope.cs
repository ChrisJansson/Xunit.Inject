using System;

namespace Xunit.Inject
{
    public interface ITestFixtureScope : IDisposable
    {
        object Fixture { get; }
    }
}