namespace Xunit.Inject
{
    public interface ITestFixtureFactory
    {
        ITestFixtureScope Create();
    }
}