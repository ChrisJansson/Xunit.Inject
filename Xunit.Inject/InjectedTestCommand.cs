using System.Xml;
using Xunit.Sdk;

namespace Xunit.Inject
{
    public class InjectedTestCommand : ITestCommand
    {
        private readonly ITestCommand _testCommand;
        private readonly ITestFixtureFactory _testFixtureFactory;

        public InjectedTestCommand(ITestCommand testCommand, ITestFixtureFactory testFixtureFactory)
        {
            _testCommand = testCommand;
            _testFixtureFactory = testFixtureFactory;
        }

        public MethodResult Execute(object testClass)
        {
            using (var testFixtureScope = _testFixtureFactory.Create())
            {
                return _testCommand.Execute(testFixtureScope.Fixture);
            }
        }

        public XmlNode ToStartXml()
        {
            return _testCommand.ToStartXml();
        }

        public string DisplayName
        {
            get { return _testCommand.DisplayName; }
        }

        public bool ShouldCreateInstance
        {
            get { return false; }
        }

        public int Timeout
        {
            get { return _testCommand.Timeout; }
        }
    }
}