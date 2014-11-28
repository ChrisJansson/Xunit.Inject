using System;
using System.Xml;
using NSubstitute;
using Xunit.Sdk;

namespace Xunit.Inject.Tests
{
    public class InjectedTestCommandTests
    {
        private readonly ITestCommand _testCommand;
        private readonly ITestFixtureFactory _testFixtureFactory;
        private readonly ITestFixtureScope _testFixtureScope;

        private readonly InjectedTestCommand _sut;

        public InjectedTestCommandTests()
        {
            _testCommand = Substitute.For<ITestCommand>();
            _testFixtureScope = Substitute.For<ITestFixtureScope>();
            _testFixtureFactory = Substitute.For<ITestFixtureFactory>();

            _sut = new InjectedTestCommand(_testCommand, _testFixtureFactory);
        }

        [Fact]
        public void Should_execute_test_command_with_created_instance_and_return_the_result()
        {
            var fakeMethodResult = new NullMethodResult();
            var testFixture = new object();
            _testFixtureScope.Fixture.Returns(testFixture);
            _testFixtureFactory.Create().Returns(_testFixtureScope);
            _testCommand.Execute(testFixture).Returns(fakeMethodResult);

            var actual = _sut.Execute(null);

            Assert.Same(fakeMethodResult, actual);
        }

        [Fact]
        public void Should_dispose_test_fixture_scope_after_execution()
        {
            _testFixtureFactory.Create().Returns(_testFixtureScope);

            _sut.Execute(null);

            _testFixtureScope.Received().Dispose();
        }

        [Fact]
        public void Should_dispose_test_fixture_scope_if_execution_fails()
        {
            _testFixtureFactory.Create().Returns(_testFixtureScope);
            _testCommand.When(x => x.Execute(Arg.Any<object>())).Do(x =>
            {
                throw new Exception();
            });

            ExecuteAndCatch();

            _testFixtureScope.Received().Dispose();
        }

        [Fact]
        public void Should_create_instance_for_xunit()
        {
            Assert.False(_sut.ShouldCreateInstance);
        }

        [Fact]
        public void Should_have_decorated_commands_timeout()
        {
            const int expectedTimeout = 123;
            _testCommand.Timeout.Returns(expectedTimeout);

            Assert.Equal(expectedTimeout, _sut.Timeout);
        }

        [Fact]
        public void Should_have_decorated_test_commands_display_name()
        {
            const string expectedDisplayName = "display name";
            _testCommand.DisplayName.Returns(expectedDisplayName);

            Assert.Equal(expectedDisplayName, _sut.DisplayName);
        }

        [Fact]
        public void Should_have_decorated_test_commands_to_start_xml()
        {
            var expectedXmlNode = new XmlDocument();
            _testCommand.ToStartXml().Returns(expectedXmlNode);

            Assert.Same(expectedXmlNode, _sut.ToStartXml());
        }

        private void ExecuteAndCatch()
        {
            try
            {
                _sut.Execute(null);
            }
            catch (Exception)
            {
                
            }
        }

        public class NullMethodResult : MethodResult
        {
            public NullMethodResult()
                : base(Substitute.For<IMethodInfo>(), "NullMethodResult") { }
        }
    }
}